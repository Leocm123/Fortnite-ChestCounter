using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace ChestCounter;

/// <summary>
/// Small always-on-top counter window drawn with plain Win32/GDI (no WinForms needed).
/// Left-drag to move it, right-click to hide it (Ctrl+Shift+O or the counter's menu shows it again).
/// Visible over Fortnite when the game runs in "windowed fullscreen" (not exclusive fullscreen).
/// </summary>
/// <summary>What the overlay shows. <see cref="Current"/> null = no match in progress.</summary>
/// <param name="Detail">Optional secondary text drawn small and grey after the value.</param>
public sealed record OverlayRow(string Label, string Value, bool Treasure = false, string? Detail = null);

/// <param name="Current">Right counter (match in progress, or last match); null = "—".</param>
/// <param name="CurrentLive">Right counter is the match in progress (green).</param>
/// <param name="Badge">Header badge ("● EN DIRECT" / "● LIVE", "● PARTIE EN COURS" / "● MATCH IN PROGRESS") or null.</param>
public sealed record OverlayView(string Total, string? Current, string CurrentCaption, bool CurrentLive, string? Badge, IReadOnlyList<OverlayRow> Rows, string Treasures = "0", string Session = "0", string? Last = null, string? Squad = null, IReadOnlyList<Goals.Progress>? Goals = null);

[SupportedOSPlatform("windows")]
public static class NativeOverlay
{
    private static Thread? _thread;
    private static IntPtr _hwnd;
    private static volatile OverlayView _view = new("0", null, OverlayModel.LastMatchCaption, false, null, Array.Empty<OverlayRow>());

    // Layout: computed by Relayout, used by Paint (both on the overlay thread).
    private static IntPtr _fontHeader, _fontCaption, _fontLabel;
    private static IntPtr _brushBorder, _brushLine;
    private static string Header => L.T("COFFRES", "CHESTS", "COFRES", "BAÚS");
    private const string CaptionTotal = "total";
    private static Action<int, int>? _onMoved;
    private static Action<bool>? _onLockChanged;
    private static volatile bool _locked;
    private static bool _offscreen;
    private static int _stableW;
    private static RECT _lockRect; // lock button, client coordinates
    private static IntPtr _fontIcon;
    private static WndProcDelegate? _wndProc; // keep the delegate alive
    private static IntPtr _fontBig, _fontSmall, _brushBg;
    private static float _scale = 1f;    // display scaling × size chosen by the player
    private static float _dpiScale = 1f; // display scaling only
    private static volatile int _sizePercent = 100;
    private static volatile int _opacityPercent = 100;

    /// <summary>"hud" (figures only, under the minimap, like the game's HUD) or "classic" (dark card).</summary>
    private static volatile string _style = "classic";
    private static IntPtr _fontHudBig, _fontHudSmall, _fontHudTiny, _fontPanelBig, _fontPanelNum;
    private static int _hudBigH, _hudSmallH;

    /// <summary>Pixels of this color are fully transparent (styles without a background). Almost black, so that the
    /// anti-aliased edge of the black outline around the text blends into it without any colored fringe.</summary>
    private static readonly uint KeyColor = 0x00010001;

    /// <summary>"hud" (aligned block), "icons" (bars with icons), "classic" (quest panel).</summary>
    public static string[] Styles => new[] { "hud", "icons", "classic" };

    /// <summary>Changes the look of the overlay at once.</summary>
    public static void SetStyle(string style)
    {
        _style = Array.IndexOf(Styles, style) >= 0 ? style : "hud";
        if (_hwnd != IntPtr.Zero)
        {
            PostMessageW(_hwnd, WM_APP_APPEARANCE, IntPtr.Zero, IntPtr.Zero);
        }
    }

    private static bool Keyed => _style != "classic";

    private const int MinWidth = 240, Pad = 16, ColGap = 36;

    /// <summary>Size and opacity of the overlay (percent). Applied at once when the overlay is shown.</summary>
    public static void SetAppearance(int sizePercent, int opacityPercent)
    {
        _sizePercent = OverlayAppearance.ClampSize(sizePercent);
        _opacityPercent = OverlayAppearance.ClampOpacity(opacityPercent);
        if (_hwnd != IntPtr.Zero)
        {
            PostMessageW(_hwnd, WM_APP_APPEARANCE, IntPtr.Zero, IntPtr.Zero);
        }
    }

    private static byte Alpha => (byte)Math.Round(_opacityPercent * 255 / 100.0);

    public static bool IsRunning => _hwnd != IntPtr.Zero;

    /// <summary>True when the overlay window is shown (not hidden with the shortcut or the menu).</summary>
    public static bool IsVisible => _hwnd != IntPtr.Zero && IsWindowVisible(_hwnd);

    /// <summary>Closes the overlay window and its thread (overlay turned off in the settings). Start() can open it again.</summary>
    public static void Stop()
    {
        var thread = _thread;
        if (thread == null)
        {
            return;
        }
        if (_hwnd != IntPtr.Zero)
        {
            PostMessageW(_hwnd, WM_APP_STOP, IntPtr.Zero, IntPtr.Zero);
        }
        thread.Join(TimeSpan.FromSeconds(3));
    }

    /// <summary>True when the global Ctrl+Shift+O shortcut could be registered.</summary>
    public static bool HotkeyRegistered { get; private set; }

    /// <summary>True when the global Ctrl+Shift+L shortcut (lock/unlock) could be registered.</summary>
    public static bool LockHotkeyRegistered { get; private set; }

    /// <summary>Locked = click-through: the mouse goes through the overlay and it cannot be moved.</summary>
    public static bool IsLocked => _locked;

    public static void Start(int? x, int? y, bool locked, Action<int, int> onMoved, Action<bool> onLockChanged)
    {
        if (_thread != null)
        {
            return;
        }

        _onMoved = onMoved;
        _offscreen = x <= -10000; // --overlay-preview: never brought back on screen
        _onLockChanged = onLockChanged;
        _locked = locked;
        var ready = new ManualResetEventSlim();
        _thread = new Thread(() =>
        {
            try
            {
                Run(x, y, ready);
            }
            catch (Exception ex)
            {
                Console.WriteLine(L.T($"Overlay indisponible : {ex.Message}", $"Overlay unavailable: {ex.Message}", $"Overlay no disponible: {ex.Message}", $"Overlay indisponível: {ex.Message}"));
                ready.Set();
            }
            finally
            {
                _hwnd = IntPtr.Zero;
                HotkeyRegistered = LockHotkeyRegistered = false;
                _thread = null;
            }
        })
        {
            IsBackground = true,
            Name = "Overlay",
        };
        _thread.SetApartmentState(ApartmentState.STA);
        _thread.Start();
        ready.Wait(TimeSpan.FromSeconds(5));
    }

    /// <summary>Replaces what is shown. The window resizes itself to fit.</summary>
    public static void Update(OverlayView view)
    {
        _view = view;
        if (_hwnd != IntPtr.Zero)
        {
            PostMessageW(_hwnd, WM_APP_RELAYOUT, IntPtr.Zero, IntPtr.Zero);
        }
    }

    /// <summary>Saves the overlay as it is drawn to a PNG file (works off-screen). Used by --overlay-preview.</summary>
    public static bool Snapshot(string path)
    {
        if (_hwnd == IntPtr.Zero || !GetWindowRect(_hwnd, out var r))
        {
            return false;
        }
        int w = r.Right - r.Left, h = r.Bottom - r.Top;
        var screen = GetDC(IntPtr.Zero);
        var mem = CreateCompatibleDC(screen);
        var bmp = CreateCompatibleBitmap(screen, w, h);
        var old = SelectObject(mem, bmp);
        var ok = PrintWindow(_hwnd, mem, 1 /* PW_CLIENTONLY: WM_PRINT -> WM_PRINTCLIENT */);
        SelectObject(mem, old);
        var bi = new BITMAPINFOHEADER { biSize = 40, biWidth = w, biHeight = -h, biPlanes = 1, biBitCount = 32 };
        var bgra = new byte[w * h * 4];
        GetDIBits(mem, bmp, 0, (uint)h, bgra, ref bi, 0);
        DeleteObject(bmp);
        DeleteDC(mem);
        ReleaseDC(IntPtr.Zero, screen);
        Png.Write(path, w, h, bgra);
        return ok;
    }

    public static void ToggleLock()
    {
        if (_hwnd != IntPtr.Zero)
        {
            PostMessageW(_hwnd, WM_APP_LOCK, IntPtr.Zero, IntPtr.Zero);
        }
    }

    public static void Toggle()
    {
        if (_hwnd != IntPtr.Zero)
        {
            PostMessageW(_hwnd, WM_APP_TOGGLE, IntPtr.Zero, IntPtr.Zero);
        }
    }

    private static void Run(int? x, int? y, ManualResetEventSlim ready)
    {
        try { SetProcessDPIAware(); } catch { /* old Windows */ }

        var screenDc = GetDC(IntPtr.Zero);
        _dpiScale = GetDeviceCaps(screenDc, LOGPIXELSY) / 96f;
        ReleaseDC(IntPtr.Zero, screenDc);

        int S(int v) => (int)Math.Round(v * _scale);

        CreateFonts();
        _brushBg = CreateSolidBrush(Rgb(14, 20, 34)); // the quest panel's plate (made translucent by the opacity)
        _brushBorder = CreateSolidBrush(Rgb(44, 56, 80));
        _brushLine = CreateSolidBrush(Rgb(44, 44, 52));

        var hInstance = GetModuleHandleW(null);
        _wndProc = WndProc;
        var wc = new WNDCLASSEXW
        {
            cbSize = (uint)Marshal.SizeOf<WNDCLASSEXW>(),
            lpfnWndProc = Marshal.GetFunctionPointerForDelegate(_wndProc),
            hInstance = hInstance,
            hCursor = LoadCursorW(IntPtr.Zero, IDC_SIZEALL),
            hbrBackground = _brushBg,
            lpszClassName = "ChestCounterOverlay",
        };
        // Already registered when the overlay is turned off and on again in the same run (ERROR_CLASS_ALREADY_EXISTS).
        if (RegisterClassExW(ref wc) == 0 && Marshal.GetLastWin32Error() != 1410)
        {
            throw new InvalidOperationException("RegisterClassEx failed");
        }

        int w = S(MinWidth), h = S(90);
        var posX = x ?? S(24);
        var posY = y ?? S(24);

        _hwnd = CreateWindowExW(
            // Opaque (no WS_EX_LAYERED alpha): nothing for the compositor to blend every frame.
            WS_EX_TOPMOST | WS_EX_TOOLWINDOW | WS_EX_NOACTIVATE,
            wc.lpszClassName, L.T("Coffres", "Chests", "Cofres", "Baús"), WS_POPUP,

            posX, posY, w, h, IntPtr.Zero, IntPtr.Zero, hInstance, IntPtr.Zero);
        if (_hwnd == IntPtr.Zero)
        {
            throw new InvalidOperationException("CreateWindowEx failed");
        }

        ApplyLock(_hwnd, _locked, notify: false);
        Relayout(_hwnd);
        ShowWindow(_hwnd, SW_SHOWNOACTIVATE);
        // No timer at all: the thread sleeps in GetMessage and only wakes up when the counter changes,
        // when you drag/hide the window or press the shortcut.
        // Global shortcut, works even when the game has the focus.
        HotkeyRegistered = RegisterHotKey(_hwnd, 1, MOD_CONTROL | MOD_SHIFT | MOD_NOREPEAT, VK_O);
        LockHotkeyRegistered = RegisterHotKey(_hwnd, 2, MOD_CONTROL | MOD_SHIFT | MOD_NOREPEAT, VK_L);
        ready.Set();

        while (GetMessageW(out var msg, IntPtr.Zero, 0, 0) > 0)
        {
            TranslateMessage(ref msg);
            DispatchMessageW(ref msg);
        }
    }

    /// <summary>(Re)creates the fonts for the current display scaling and chosen size.</summary>
    private static void CreateFonts()
    {
        foreach (var old in new[] { _fontBig, _fontSmall, _fontHeader, _fontCaption, _fontLabel, _fontIcon, _fontHudBig, _fontHudSmall, _fontHudTiny, _fontPanelBig, _fontPanelNum })
        {
            if (old != IntPtr.Zero) DeleteObject(old);
        }
        _scale = _dpiScale * _sizePercent / 100f;
        int S(int v) => (int)Math.Round(v * _scale);
        _fontBig = CreateFontW(-S(38), 0, 0, 0, 800, 0, 0, 0, 1, 0, 0, 5, 0, "Segoe UI");
        _fontSmall = CreateFontW(-S(14), 0, 0, 0, 500, 0, 0, 0, 1, 0, 0, 5, 0, "Segoe UI");
        _fontHeader = CreateFontW(-S(12), 0, 0, 0, 700, 0, 0, 0, 1, 0, 0, 5, 0, "Segoe UI");
        _fontCaption = CreateFontW(-S(11), 0, 0, 0, 600, 0, 0, 0, 1, 0, 0, 5, 0, "Segoe UI");
        _fontLabel = CreateFontW(-S(13), 0, 0, 0, 600, 0, 0, 0, 1, 0, 0, 5, 0, "Segoe UI");
        _fontIcon = CreateFontW(-S(14), 0, 0, 0, 400, 0, 0, 0, 1, 0, 0, 5, 0, "Segoe MDL2 Assets");
        // Game-like styles: condensed italic, grayscale anti-aliasing (ClearType would leave colored edges once the
        // background is made transparent).
        _fontHudBig = CreateFontW(-S(40), 0, 0, 0, 700, 1, 0, 0, 1, 0, 0, 4, 0, "Bahnschrift Condensed");
        _fontHudSmall = CreateFontW(-S(18), 0, 0, 0, 600, 1, 0, 0, 1, 0, 0, 4, 0, "Bahnschrift SemiCondensed");
        _fontHudTiny = CreateFontW(-S(14), 0, 0, 0, 700, 1, 0, 0, 1, 0, 0, 4, 0, "Bahnschrift SemiCondensed");
        // Quest panel: clean, upright figures.
        _fontPanelBig = CreateFontW(-S(30), 0, 0, 0, 700, 0, 0, 0, 1, 0, 0, 5, 0, "Bahnschrift");
        _fontPanelNum = CreateFontW(-S(15), 0, 0, 0, 700, 0, 0, 0, 1, 0, 0, 5, 0, "Bahnschrift");
    }

    private static IntPtr WndProc(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam)
    {
        switch (msg)
        {
            case WM_NCHITTEST:
            {
                // Lock button = normal client area (clickable); everywhere else = caption (drag to move).
                // When locked the window is WS_EX_TRANSPARENT, so it never even receives this message.
                GetWindowRect(hwnd, out var wr);
                var px = (short)((long)lParam & 0xFFFF) - wr.Left;
                var py = (short)(((long)lParam >> 16) & 0xFFFF) - wr.Top;
                var onButton = px >= _lockRect.Left && px < _lockRect.Right && py >= _lockRect.Top && py < _lockRect.Bottom;
                return (IntPtr)(onButton ? HTCLIENT : HTCAPTION);
            }

            case WM_SETCURSOR:
                if (((long)lParam & 0xFFFF) == HTCLIENT)
                {
                    SetCursor(LoadCursorW(IntPtr.Zero, IDC_HAND));
                    return (IntPtr)1;
                }
                break;

            case WM_LBUTTONUP:
                ApplyLock(hwnd, true, notify: true);
                return IntPtr.Zero;

            case WM_APP_LOCK:
                ApplyLock(hwnd, !_locked, notify: true);
                return IntPtr.Zero;

            case WM_NCRBUTTONUP:
                SetVisible(hwnd, false);
                return IntPtr.Zero;

            case WM_HOTKEY:
                if ((long)wParam == 2)
                {
                    ApplyLock(hwnd, !_locked, notify: true);
                }
                else
                {
                    SetVisible(hwnd, !IsWindowVisible(hwnd));
                }
                return IntPtr.Zero;

            case WM_EXITSIZEMOVE:
                if (GetWindowRect(hwnd, out var r))
                {
                    _onMoved?.Invoke(r.Left, r.Top);
                }
                return IntPtr.Zero;

            case WM_APP_STOP:
                DestroyWindow(hwnd);
                return IntPtr.Zero;

            case WM_APP_TOGGLE:
                SetVisible(hwnd, !IsWindowVisible(hwnd));
                return IntPtr.Zero;

            case WM_APP_RELAYOUT:
                Relayout(hwnd);
                return IntPtr.Zero;

            case WM_APP_APPEARANCE:
                CreateFonts();
                _stableW = 0; // the width may shrink when the overlay gets smaller
                ApplyLock(hwnd, _locked, notify: false); // also applies the opacity
                Relayout(hwnd);
                return IntPtr.Zero;

            case WM_PAINT:
                Paint(hwnd);
                return IntPtr.Zero;

            case WM_PRINTCLIENT: // snapshot (--overlay-preview): draw into the given DC
                PaintTo(hwnd, wParam);
                return IntPtr.Zero;

            case WM_DESTROY:
                UnregisterHotKey(hwnd, 1);
                UnregisterHotKey(hwnd, 2);
                PostQuitMessage(0);
                return IntPtr.Zero;
        }
        return DefWindowProcW(hwnd, msg, wParam, lParam);
    }

    /// <summary>
    /// Locked: click-through (WS_EX_LAYERED + WS_EX_TRANSPARENT). Unlocked: normal window. Layered also when the
    /// player made it see-through; a fully opaque unlocked overlay stays a plain window (nothing to blend).
    /// </summary>
    private static void ApplyLock(IntPtr hwnd, bool locked, bool notify)
    {
        _locked = locked;
        var alpha = Alpha;
        var layered = locked || alpha < 255 || Keyed;
        var ex = (long)GetWindowLongPtrW(hwnd, GWL_EXSTYLE);
        ex = layered ? ex | WS_EX_LAYERED : ex & ~(long)WS_EX_LAYERED;
        ex = locked ? ex | WS_EX_TRANSPARENT : ex & ~(long)WS_EX_TRANSPARENT;
        SetWindowLongPtrW(hwnd, GWL_EXSTYLE, (IntPtr)ex);
        if (layered)
        {
            // Required for a layered window to be drawn; the key color makes the background of the game-like styles invisible.
            SetLayeredWindowAttributes(hwnd, KeyColor, alpha, Keyed ? LWA_ALPHA | LWA_COLORKEY : LWA_ALPHA);
        }
        SetWindowPos(hwnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE | SWP_FRAMECHANGED);
        InvalidateRect(hwnd, IntPtr.Zero, true);

        if (notify)
        {
            _onLockChanged?.Invoke(locked);
            Console.WriteLine(locked
                ? L.T($"Overlay verrouillé : la souris passe au travers. {(LockHotkeyRegistered ? "Ctrl+Maj+L" : "« Déverrouiller l'overlay » dans le compteur")} pour le déverrouiller.",
                      $"Overlay locked: the mouse goes through it. {(LockHotkeyRegistered ? "Ctrl+Shift+L" : "\"Unlock the overlay\" in the counter")} to unlock it.",
                      $"Overlay bloqueado: el ratón lo atraviesa. {(LockHotkeyRegistered ? "Ctrl+Mayús+L" : "«Desbloquear el overlay» en el contador")} para desbloquearlo.", $"Overlay travado: o mouse passa através dele. {(LockHotkeyRegistered ? "Ctrl+Shift+L" : "\"Destravar para mover\" no contador")} para destravar.")
                : L.T("Overlay déverrouillé : clic gauche maintenu pour le déplacer, cadenas pour le reverrouiller.",
                      "Overlay unlocked: hold the left mouse button to move it, click the padlock to lock it again.",
                      "Overlay desbloqueado: arrástralo con el clic izquierdo; el candado lo vuelve a bloquear.", "Overlay destravado: segure o botão esquerdo do mouse para mover, clique no cadeado para travar de novo."));
        }
    }

    private static void SetVisible(IntPtr hwnd, bool visible)
    {
        if (visible)
        {
            ShowWindow(hwnd, SW_SHOWNOACTIVATE);
            SetWindowPos(hwnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
        }
        else
        {
            ShowWindow(hwnd, SW_HIDE);
            Console.WriteLine(HotkeyRegistered
                ? L.T("Overlay masqué : Ctrl+Maj+O (n'importe où) pour le réafficher.", "Overlay hidden: Ctrl+Shift+O (anywhere) to show it again.", "Overlay oculto: Ctrl+Mayús+O (desde cualquier sitio) para volver a mostrarlo.", "Overlay escondido: Ctrl+Shift+O (em qualquer lugar) para mostrar de novo.")
                : L.T("Overlay masqué : « Afficher l'overlay » dans le compteur pour le réafficher.", "Overlay hidden: \"Show the overlay\" in the counter to show it again.", "Overlay oculto: «Mostrar el overlay» en el contador para volver a mostrarlo.", "Overlay escondido: \"Mostrar o overlay\" no contador para mostrar de novo."));
        }
    }

    private static int TextWidth(IntPtr hdc, IntPtr font, string text, out int height)
    {
        SelectObject(hdc, font);
        var r = new RECT();
        DrawTextW(hdc, text.Length == 0 ? " " : text, -1, ref r, DT_CALCRECT | DT_SINGLELINE | DT_NOPREFIX);
        height = r.Bottom - r.Top;
        return r.Right - r.Left;
    }

    // ---- Three looks, picked in the Overlay tab -----------------------------------------------------------------------
    //  "hud"     Aligned block: outlined figures like the game's HUD, the goals as bars of the same width in a column.
    //  "icons"   Bars with icons: the same figures, the goals as an icon and a bar with its number inside.
    //  "classic" Quest panel: a translucent dark plate, like the game's quest tracker, clean white text, thin bars.

    /// <summary>The small facts under the total: last match, session, squad (when shown), treasures.</summary>
    private sealed record Fact(string Label, string Value, bool Treasure = false);

    private static List<Fact> Facts(OverlayView v)
    {
        var facts = new List<Fact>
        {
            new(L.T("Dernière", "Last", "Última", "Última"), v.Last ?? "—"),
            new(L.T("Session", "Session", "Sesión", "Sessão"), v.Session),
        };
        if (v.Squad != null)
        {
            facts.Add(new(OverlayModel.SquadLabel, v.Squad));
        }
        facts.Add(new(L.T("Trésors", "Treasures", "Tesoros", "Tesouros"), v.Treasures, Treasure: true));
        return facts;
    }

    private static uint Green => Rgb(63, 220, 138);
    private static uint Ink => Rgb(11, 15, 24);

    /// <summary>The figures of the match in progress are readable (its chests are already in the total).</summary>
    private static bool LiveFigures(OverlayView v) => v.CurrentLive && v.Current is { } cur && cur != "…";

    private static int Sc(double v) => (int)Math.Round(v * _scale);

    // Layout of the HUD styles (computed by Relayout, used by Paint).
    private static int _goalRowH, _goalLabelW, _goalNumW;
    private const int HudBarW = 118, IconBarW = 150;

    /// <summary>Measures the content and resizes the window to fit it (it never shrinks while the look stays the same).</summary>
    private static void Relayout(IntPtr hwnd)
    {
        var hdc = GetDC(hwnd);
        int w, h;
        try
        {
            (w, h) = Keyed ? MeasureHud(hdc) : MeasurePanel(hdc);
        }
        finally
        {
            ReleaseDC(hwnd, hdc);
        }
        w = Math.Max(_stableW, w);
        _stableW = w;
        if (Keyed)
        {
            _lockRect = new RECT { Left = Sc(2), Top = 0, Right = Sc(22), Bottom = h };
        }
        else
        {
            _lockRect = new RECT { Left = w - Sc(30), Top = Sc(6), Right = w - Sc(6), Bottom = Sc(30) };
        }

        // Kept fully on its screen: on the right half it grows to the left (under the minimap), and it is pushed back
        // inside when it would overflow.
        GetWindowRect(hwnd, out var cur);
        int nx = cur.Left, ny = cur.Top;
        var mi = new MONITORINFO { cbSize = (uint)Marshal.SizeOf<MONITORINFO>() };
        if (!_offscreen && GetMonitorInfoW(MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST), ref mi))
        {
            var work = mi.rcWork;
            if (cur.Right - cur.Left > 0 && (cur.Left + cur.Right) / 2 > (work.Left + work.Right) / 2)
            {
                nx = cur.Right - w;
            }
            nx = Math.Max(work.Left, Math.Min(nx, work.Right - w));
            ny = Math.Max(work.Top, Math.Min(ny, work.Bottom - h));
        }
        SetWindowPos(hwnd, HWND_TOPMOST, nx, ny, w, h, SWP_NOACTIVATE);
        // The panel has rounded corners; the HUD looks are a plain rectangle whose background is made invisible.
        SetWindowRgn(hwnd, Keyed ? IntPtr.Zero : CreateRoundRectRgn(0, 0, w + 1, h + 1, Sc(12), Sc(12)), true);
        if (nx != cur.Left || ny != cur.Top)
        {
            _onMoved?.Invoke(nx, ny);
        }
        InvalidateRect(hwnd, IntPtr.Zero, true);
        UpdateWindow(hwnd); // paint now (one repaint per update, no timer)
    }

    private static (int W, int H) MeasureHud(IntPtr hdc)
    {
        var v = _view;
        var pad = Sc(8);
        var bigW = TextWidth(hdc, _fontHudBig, v.Total, out _hudBigH);
        var topW = Sc(18) + Sc(26) + Sc(8) + bigW; // room for the in-match dot is always kept
        var chipsW = FactsWidth(hdc, Facts(v), out _hudSmallH);
        var goals = v.Goals ?? [];
        var goalsW = 0;
        if (goals.Count > 0)
        {
            if (_style == "icons")
            {
                _goalRowH = Math.Max(Sc(22), Sc(20));
                goalsW = Sc(22) + Sc(7) + Sc(IconBarW);
            }
            else
            {
                _goalLabelW = goals.Max(g => TextWidth(hdc, _fontHudSmall, g.Short, out _));
                _goalNumW = goals.Max(g => TextWidth(hdc, _fontHudSmall, g.Numbers, out _));
                _goalRowH = _hudSmallH;
                goalsW = _goalLabelW + Sc(8) + Sc(HudBarW) + Sc(8) + _goalNumW;
            }
        }
        var w = Sc(22) + pad + Math.Max(topW, Math.Max(chipsW, goalsW)) + pad + Sc(14); // italic glyphs lean right
        var h = pad + _hudBigH + Sc(2) + _hudSmallH + (goals.Count > 0 ? Sc(6) + goals.Count * (_goalRowH + Sc(5)) : 0) + pad;
        return (w, h);
    }

    /// <summary>"Last 6  Session 31  ★ 1": its width, and the height of the line.</summary>
    private static int FactsWidth(IntPtr hdc, List<Fact> facts, out int height)
    {
        height = 0;
        var w = 0;
        foreach (var f in facts)
        {
            w += TextWidth(hdc, _fontHudSmall, f.Value, out var hh) + (f.Treasure ? Sc(18) : TextWidth(hdc, _fontHudSmall, f.Label, out _) + Sc(6));
            height = Math.Max(height, hh);
        }
        return w + (facts.Count - 1) * Sc(12);
    }

    private static int _panelTop, _panelFactsH, _panelGoalH, _panelNumH;

    private static (int W, int H) MeasurePanel(IntPtr hdc)
    {
        var v = _view;
        var facts = Facts(v);
        var totalW = Sc(26) + Sc(8) + TextWidth(hdc, _fontPanelBig, v.Total, out _panelTop) + Sc(10) + TextWidth(hdc, _fontCaption, CaptionTotalLine1, out _) + Sc(30);
        var factsW = facts.Sum(f => TextWidth(hdc, _fontCaption, f.Label, out _) + Sc(4) + TextWidth(hdc, _fontPanelNum, f.Value, out _)) + (facts.Count - 1) * Sc(14);
        TextWidth(hdc, _fontPanelNum, "0", out _panelNumH);
        _panelFactsH = _panelNumH;
        var goals = v.Goals ?? [];
        var goalsW = goals.Count == 0 ? 0 : goals.Max(g => TextWidth(hdc, _fontLabel, g.Label, out _) + Sc(16) + TextWidth(hdc, _fontPanelNum, g.Numbers, out _));
        _panelGoalH = _panelNumH + Sc(4) + Sc(7);
        var inner = Math.Max(Sc(212), Math.Max(totalW, Math.Max(factsW, goalsW)));
        var h = Sc(12) + _panelTop + Sc(8) + _panelFactsH + goals.Count * (Sc(9) + _panelGoalH) + Sc(14);
        return (inner + 2 * Sc(14), h);
    }

    private static string CaptionTotalLine1 => L.T("coffres", "chests", "cofres", "baús");
    private static string CaptionTotalLine2 => L.T("au total", "in total", "en total", "no total");

    private static void Text(IntPtr hdc, IntPtr font, uint color, string text, int left, int top, int right, int bottom, uint align = DT_LEFT)
    {
        SelectObject(hdc, font);
        SetTextColor(hdc, color);
        var r = new RECT { Left = left, Top = top, Right = right, Bottom = bottom };
        DrawTextW(hdc, text, -1, ref r, DT_SINGLELINE | DT_NOPREFIX | align);
    }

    private static void Paint(IntPtr hwnd)
    {
        var hdc = BeginPaint(hwnd, out var ps);
        try
        {
            PaintTo(hwnd, hdc);
        }
        finally
        {
            EndPaint(hwnd, ref ps);
        }
    }

    private static void PaintTo(IntPtr hwnd, IntPtr hdc)
    {
        if (Keyed)
        {
            PaintHud(hwnd, hdc);
        }
        else
        {
            PaintPanel(hwnd, hdc);
        }
    }

    /// <summary>A rounded bar: dark track, the part done in the accent color (green once the goal is reached).</summary>
    private static void Bar(IntPtr hdc, int left, int top, int width, int height, int percent, bool done, bool outlined, uint track)
    {
        var pen = CreatePen(0, outlined ? Math.Max(1, Sc(2)) : 0, outlined ? Ink : track);
        var oldPen = SelectObject(hdc, pen);
        var brush = CreateSolidBrush(track);
        var old = SelectObject(hdc, brush);
        RoundRect(hdc, left, top, left + width, top + height, height, height);
        var fillW = (int)Math.Round(width * Math.Clamp(percent, 0, 100) / 100.0);
        if (fillW > 0)
        {
            var fill = CreateSolidBrush(done ? Green : Rgb(Themes.Current.Accent));
            SelectObject(hdc, fill);
            RoundRect(hdc, left, top, left + Math.Max(height, fillW), top + height, height, height);
            SelectObject(hdc, brush);
            DeleteObject(fill);
        }
        SelectObject(hdc, old);
        SelectObject(hdc, oldPen);
        DeleteObject(brush);
        DeleteObject(pen);
    }

    private static void PaintHud(IntPtr hwnd, IntPtr hdc)
    {
        GetClientRect(hwnd, out var rc);
        var key = CreateSolidBrush(KeyColor);
        FillRect(hdc, ref rc, key);
        DeleteObject(key);
        SetBkMode(hdc, TRANSPARENT);
        var v = _view;
        var pad = Sc(8);
        var right = rc.Right - pad - Sc(12); // italic glyphs and their outline lean past the measured width
        var white = Rgb(255, 255, 255);
        var gold = Rgb(Themes.Current.Accent);
        var labelColor = Rgb(Themes.Current.Label);

        // Unlocked: a dark backdrop to grab it, and the padlock (locked, only the figures remain, mouse goes through).
        if (!_locked)
        {
            var pen = CreatePen(0, 1, Rgb(58, 66, 86));
            var brush = CreateSolidBrush(Rgb(28, 34, 48));
            var oldPen = SelectObject(hdc, pen);
            var old = SelectObject(hdc, brush);
            RoundRect(hdc, 0, 0, rc.Right, rc.Bottom, Sc(12), Sc(12));
            SelectObject(hdc, old);
            SelectObject(hdc, oldPen);
            DeleteObject(brush);
            DeleteObject(pen);
            SelectObject(hdc, _fontIcon);
            SetTextColor(hdc, Rgb(200, 200, 208));
            var lr = _lockRect;
            DrawTextW(hdc, "", -1, ref lr, DT_SINGLELINE | DT_CENTER | DT_VCENTER | DT_NOPREFIX);
        }

        var left = _lockRect.Right;
        var y = pad;

        // Total, with the chest, and the red dot while a match is in progress.
        var bigW = TextWidth(hdc, _fontHudBig, v.Total, out _);
        Outlined(hdc, _fontHudBig, LiveFigures(v) ? gold : white, v.Total, left, y, right, y + _hudBigH, DT_RIGHT);
        var iw = Sc(26);
        var ih = Sc(22);
        var ix = right - bigW - Sc(8) - iw;
        Chest(hdc, ix, y + (_hudBigH - ih) / 2, iw, ih, gold, Rgb(Themes.Current.AccentDeep), Ink, Ink);
        if (v.CurrentLive)
        {
            var d = Sc(11);
            Disc(hdc, ix - Sc(7) - d, y + (_hudBigH - d) / 2, d, Rgb(255, 59, 48), Ink, Math.Max(1, Sc(1.5)));
        }
        y += _hudBigH + Sc(2);

        // "Last 6  Session 31  ★ 1", right-aligned, drawn from the right.
        var x = right;
        var facts = Facts(v);
        for (var i = facts.Count - 1; i >= 0; i--)
        {
            var f = facts[i];
            var vw = TextWidth(hdc, _fontHudSmall, f.Value, out _);
            Outlined(hdc, _fontHudSmall, f.Treasure ? Rgb(214, 190, 255) : white, f.Value, left, y, x, y + _hudSmallH, DT_RIGHT, thin: true);
            x -= vw;
            if (f.Treasure)
            {
                Star(hdc, x - Sc(9), y + _hudSmallH / 2, Sc(7), Rgb(196, 155, 255), Ink);
                x -= Sc(18);
            }
            else
            {
                x -= Sc(6);
                Outlined(hdc, _fontHudSmall, labelColor, f.Label, left, y, x, y + _hudSmallH, DT_RIGHT, thin: true);
                x -= TextWidth(hdc, _fontHudSmall, f.Label, out _);
            }
            x -= Sc(12);
        }
        y += _hudSmallH + Sc(6);

        foreach (var g in v.Goals ?? [])
        {
            if (_style == "icons")
            {
                // [icon] (bar with its number inside)
                var barW = Sc(IconBarW);
                var barH = Sc(20);
                var barLeft = right - barW;
                var top = y + (_goalRowH - barH) / 2;
                Bar(hdc, barLeft, top, barW, barH, g.Percent, g.Done, outlined: true, Rgb(24, 28, 38));
                Outlined(hdc, _fontHudTiny, white, g.Numbers, barLeft, top, barLeft + barW, top + barH, DT_CENTER | DT_VCENTER, thin: true);
                var d = Sc(22);
                var cx = barLeft - Sc(7) - d;
                Disc(hdc, cx, y + (_goalRowH - d) / 2, d, Ink, Ink, 1);
                GoalIcon(hdc, cx + d / 2, y + _goalRowH / 2, Sc(6), g.Label == Goals.DailyLabel, labelColor);
            }
            else
            {
                // Label | bar | number, in three aligned columns.
                var numLeft = right - _goalNumW;
                var barW = Sc(HudBarW);
                var barH = Sc(10);
                var barLeft = numLeft - Sc(8) - barW;
                Bar(hdc, barLeft, y + (_goalRowH - barH) / 2, barW, barH, g.Percent, g.Done, outlined: true, Rgb(24, 28, 38));
                Outlined(hdc, _fontHudSmall, g.Done ? Green : white, g.Numbers, numLeft, y, right + Sc(12), y + _goalRowH, DT_LEFT, thin: true);
                Outlined(hdc, _fontHudSmall, labelColor, g.Short, left, y, barLeft - Sc(8), y + _goalRowH, DT_RIGHT, thin: true);
            }
            y += _goalRowH + Sc(5);
        }
    }

    /// <summary>The goal's icon: a target (day) or a calendar (week), drawn with lines.</summary>
    private static void GoalIcon(IntPtr hdc, int cx, int cy, int r, bool day, uint color)
    {
        var pen = CreatePen(0, Math.Max(1, Sc(1.6)), color);
        var oldPen = SelectObject(hdc, pen);
        var oldBrush = SelectObject(hdc, GetStockObject(NULL_BRUSH));
        if (day)
        {
            Ellipse(hdc, cx - r, cy - r, cx + r + 1, cy + r + 1);
            var r2 = Math.Max(1, r / 2);
            Ellipse(hdc, cx - r2, cy - r2, cx + r2 + 1, cy + r2 + 1);
        }
        else
        {
            Rectangle(hdc, cx - r, cy - r + Sc(1), cx + r + 1, cy + r + 1);
            MoveToEx(hdc, cx - r, cy - r / 3, IntPtr.Zero);
            LineTo(hdc, cx + r, cy - r / 3);
        }
        SelectObject(hdc, oldBrush);
        SelectObject(hdc, oldPen);
        DeleteObject(pen);
    }

    private static void PaintPanel(IntPtr hwnd, IntPtr hdc)
    {
        GetClientRect(hwnd, out var rc);
        FillRect(hdc, ref rc, _brushBg);
        SetBkMode(hdc, TRANSPARENT);
        var v = _view;
        var border = CreateRoundRectRgn(0, 0, rc.Right + 1, rc.Bottom + 1, Sc(12), Sc(12));
        FrameRgn(hdc, border, _brushBorder, 1, 1);
        DeleteObject(border);

        var x = Sc(14);
        var right = rc.Right - Sc(14);
        var muted = Rgb(185, 196, 214);
        var white = Rgb(255, 255, 255);
        var y = Sc(12);

        // Lock button (Segoe MDL2 Assets: E785 = open padlock, E72E = closed padlock).
        SelectObject(hdc, _fontIcon);
        SetTextColor(hdc, _locked ? Rgb(96, 104, 120) : Rgb(210, 216, 228));
        var lr = _lockRect;
        DrawTextW(hdc, _locked ? "" : "", -1, ref lr, DT_SINGLELINE | DT_CENTER | DT_VCENTER | DT_NOPREFIX);

        // [chest] 290            chests
        //                        in total
        var iw = Sc(26);
        var ih = Sc(22);
        Chest(hdc, x, y + (_panelTop - ih) / 2, iw, ih, Rgb(Themes.Current.Accent), Rgb(Themes.Current.AccentDeep), Ink, Ink);
        var tx = x + iw + Sc(8);
        Text(hdc, _fontPanelBig, LiveFigures(v) ? Rgb(Themes.Current.Accent) : white, v.Total, tx, y, right, y + _panelTop);
        if (v.CurrentLive)
        {
            var d = Sc(9);
            var dx = tx + TextWidth(hdc, _fontPanelBig, v.Total, out _) + Sc(7);
            Disc(hdc, dx, y + (_panelTop - d) / 2, d, Rgb(255, 59, 48), Rgb(14, 20, 34), 1);
        }
        var capRight = _lockRect.Left - Sc(6);
        Text(hdc, _fontCaption, muted, CaptionTotalLine1, x, y + _panelTop / 2 - Sc(13), capRight, y + _panelTop / 2, DT_RIGHT);
        Text(hdc, _fontCaption, muted, CaptionTotalLine2, x, y + _panelTop / 2, capRight, y + _panelTop / 2 + Sc(13), DT_RIGHT);
        y += _panelTop + Sc(8);

        // Last 6   Session 31   Treasures 1, spread over the width.
        var facts = Facts(v);
        var widths = facts.Select(f => TextWidth(hdc, _fontCaption, f.Label, out _) + Sc(4) + TextWidth(hdc, _fontPanelNum, f.Value, out _)).ToList();
        var gap = facts.Count > 1 ? (right - x - widths.Sum()) / (facts.Count - 1) : 0;
        var fx = x;
        for (var i = 0; i < facts.Count; i++)
        {
            var f = facts[i];
            var lw = TextWidth(hdc, _fontCaption, f.Label, out _);
            Text(hdc, _fontCaption, muted, f.Label, fx, y, fx + lw + Sc(2), y + _panelFactsH, DT_VCENTER);
            Text(hdc, _fontPanelNum, f.Treasure ? Rgb(214, 190, 255) : white, f.Value, fx + lw + Sc(4), y, fx + widths[i] + Sc(4), y + _panelFactsH, DT_VCENTER);
            fx += widths[i] + gap;
        }
        y += _panelFactsH;

        // Goal: "Daily goal ........ 44 / 30" and a thin bar under it.
        foreach (var g in v.Goals ?? [])
        {
            y += Sc(9);
            Text(hdc, _fontLabel, white, g.Label, x, y, right, y + _panelNumH, DT_VCENTER);
            Text(hdc, _fontPanelNum, g.Done ? Green : white, g.Numbers, x, y, right, y + _panelNumH, DT_RIGHT | DT_VCENTER);
            y += _panelNumH + Sc(4);
            var track = new RECT { Left = x, Top = y, Right = right, Bottom = y + Sc(7) };
            var trackBrush = CreateSolidBrush(Rgb(52, 60, 78));
            FillRect(hdc, ref track, trackBrush);
            DeleteObject(trackBrush);
            var fill = new RECT { Left = x, Top = y, Right = x + (int)Math.Round((right - x) * g.Percent / 100.0), Bottom = y + Sc(7) };
            var fillBrush = CreateSolidBrush(g.Done ? Green : Rgb(Themes.Current.Accent));
            FillRect(hdc, ref fill, fillBrush);
            DeleteObject(fillBrush);
            y += Sc(7);
        }
    }

    /// <summary>Text outlined in near black, like the game's own HUD figures (readable on any background).</summary>
    private static void Outlined(IntPtr hdc, IntPtr font, uint color, string text, int left, int top, int right, int bottom, uint align, bool thin = false)
    {
        SelectObject(hdc, font);
        var o = thin ? 1 : Math.Max(1, Sc(1.6));
        SetTextColor(hdc, Rgb(11, 15, 24));
        for (var dx = -o; dx <= o; dx++)
        {
            for (var dy = -o; dy <= o + 1; dy++)
            {
                if (Math.Abs(dx) != o && Math.Abs(dy) < o) continue; // only the ring (and one pixel of drop shadow)
                var r = new RECT { Left = left + dx, Top = top + dy, Right = right + dx, Bottom = bottom + dy };
                DrawTextW(hdc, text, -1, ref r, DT_SINGLELINE | DT_NOPREFIX | DT_NOCLIP | align);
            }
        }
        SetTextColor(hdc, color);
        var rc = new RECT { Left = left, Top = top, Right = right, Bottom = bottom };
        DrawTextW(hdc, text, -1, ref rc, DT_SINGLELINE | DT_NOPREFIX | DT_NOCLIP | align); // italic glyphs lean past the box
    }

    /// <summary>A treasure chest made of three shapes: body, rounded lid, latch.</summary>
    private static void Chest(IntPtr hdc, int x, int y, int w, int h, uint body, uint lid, uint latch, uint outline)
    {
        var pen = CreatePen(0, Math.Max(1, Sc(1.5)), outline);
        var oldPen = SelectObject(hdc, pen);
        var lidH = (int)(h * 0.42);
        var bLid = CreateSolidBrush(lid);
        var old = SelectObject(hdc, bLid);
        Chord(hdc, x, y, x + w, y + 2 * lidH, x + w, y + lidH, x, y + lidH);
        var bBody = CreateSolidBrush(body);
        SelectObject(hdc, bBody);
        RoundRect(hdc, x, y + lidH, x + w, y + h, Sc(4), Sc(4));
        var bLatch = CreateSolidBrush(latch);
        SelectObject(hdc, bLatch);
        var lw = Math.Max(3, w / 5);
        Rectangle(hdc, x + (w - lw) / 2, y + lidH - Sc(2), x + (w + lw) / 2, y + lidH + Math.Max(4, h / 4));
        SelectObject(hdc, old);
        SelectObject(hdc, oldPen);
        DeleteObject(bLid);
        DeleteObject(bBody);
        DeleteObject(bLatch);
        DeleteObject(pen);
    }

    /// <summary>A five-pointed star (treasures), outlined like the text.</summary>
    private static void Star(IntPtr hdc, int cx, int cy, int r, uint fill, uint outline)
    {
        var pts = new POINT[10];
        for (var i = 0; i < 10; i++)
        {
            var a = -Math.PI / 2 + i * Math.PI / 5;
            var rr = i % 2 == 0 ? r : r * 0.45;
            pts[i] = new POINT { X = cx + (int)Math.Round(rr * Math.Cos(a)), Y = cy + (int)Math.Round(rr * Math.Sin(a)) };
        }
        var pen = CreatePen(0, Math.Max(1, Sc(1.5)), outline);
        var brush = CreateSolidBrush(fill);
        var oldPen = SelectObject(hdc, pen);
        var old = SelectObject(hdc, brush);
        Polygon(hdc, pts, pts.Length);
        SelectObject(hdc, old);
        SelectObject(hdc, oldPen);
        DeleteObject(brush);
        DeleteObject(pen);
    }

    private static void Disc(IntPtr hdc, int x, int y, int d, uint fill, uint outline, int penW)
    {
        var pen = CreatePen(0, penW, outline);
        var brush = CreateSolidBrush(fill);
        var oldPen = SelectObject(hdc, pen);
        var old = SelectObject(hdc, brush);
        Ellipse(hdc, x, y, x + d, y + d);
        SelectObject(hdc, old);
        SelectObject(hdc, oldPen);
        DeleteObject(brush);
        DeleteObject(pen);
    }

    private static uint Rgb(byte r, byte g, byte b) => (uint)(r | (g << 8) | (b << 16));
    private static uint Rgb((byte R, byte G, byte B) c) => Rgb(c.R, c.G, c.B);

    // ---- Win32 ------------------------------------------------------------------------------
    private delegate IntPtr WndProcDelegate(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam);

    private const uint WS_POPUP = 0x80000000;
    private const uint WS_EX_TOPMOST = 0x00000008, WS_EX_TRANSPARENT = 0x00000020, WS_EX_TOOLWINDOW = 0x00000080, WS_EX_LAYERED = 0x00080000, WS_EX_NOACTIVATE = 0x08000000;
    private const int GWL_EXSTYLE = -20, HTCLIENT = 1;
    private const uint WM_SETCURSOR = 0x0020, WM_LBUTTONUP = 0x0202, WM_APP_LOCK = 0x8003, VK_L = 0x4C, SWP_FRAMECHANGED = 0x20;
    private static readonly IntPtr IDC_HAND = new(32649);
    private const uint LWA_ALPHA = 0x2, LWA_COLORKEY = 0x1;
    private const int SW_HIDE = 0, SW_SHOWNOACTIVATE = 4;
    private const uint WM_DESTROY = 0x0002, WM_PAINT = 0x000F, WM_NCHITTEST = 0x0084, WM_NCRBUTTONUP = 0x00A5,
        WM_TIMER = 0x0113, WM_EXITSIZEMOVE = 0x0232, WM_APP_TOGGLE = 0x8001, WM_PRINTCLIENT = 0x0318, WM_APP_RELAYOUT = 0x8002, WM_HOTKEY = 0x0312, WM_APP_APPEARANCE = 0x8004, WM_APP_STOP = 0x8005;
    private const uint MOD_CONTROL = 0x2, MOD_SHIFT = 0x4, MOD_NOREPEAT = 0x4000, VK_O = 0x4F;
    private const int HTCAPTION = 2;
    private const int TRANSPARENT = 1, LOGPIXELSY = 90;
    private const uint DT_LEFT = 0x0, DT_CENTER = 0x1, DT_RIGHT = 0x2, DT_VCENTER = 0x4, DT_BOTTOM = 0x8, DT_SINGLELINE = 0x20, DT_CALCRECT = 0x400, DT_NOPREFIX = 0x800, DT_NOCLIP = 0x100;
    private const uint SWP_NOSIZE = 0x1, SWP_NOMOVE = 0x2, SWP_NOACTIVATE = 0x10;
    private static readonly IntPtr HWND_TOPMOST = new(-1);
    private static readonly IntPtr IDC_SIZEALL = new(32646);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct WNDCLASSEXW
    {
        public uint cbSize, style;
        public IntPtr lpfnWndProc;
        public int cbClsExtra, cbWndExtra;
        public IntPtr hInstance, hIcon, hCursor, hbrBackground;
        public string? lpszMenuName;
        public string lpszClassName;
        public IntPtr hIconSm;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct RECT { public int Left, Top, Right, Bottom; }

    [StructLayout(LayoutKind.Sequential)]
    private struct POINT { public int X, Y; }

    [StructLayout(LayoutKind.Sequential)]
    private struct MSG
    {
        public IntPtr hwnd;
        public uint message;
        public IntPtr wParam, lParam;
        public uint time;
        public POINT pt;
        public uint lPrivate;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct PAINTSTRUCT
    {
        public IntPtr hdc;
        public int fErase;
        public RECT rcPaint;
        public int fRestore, fIncUpdate;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)] public byte[] rgbReserved;
    }

    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)] private static extern ushort RegisterClassExW(ref WNDCLASSEXW wc);
    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern IntPtr CreateWindowExW(uint exStyle, string className, string windowName, uint style, int x, int y, int w, int h, IntPtr parent, IntPtr menu, IntPtr instance, IntPtr param);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] private static extern IntPtr DefWindowProcW(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] private static extern int GetMessageW(out MSG msg, IntPtr hwnd, uint min, uint max);
    [DllImport("user32.dll")] private static extern bool TranslateMessage(ref MSG msg);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] private static extern IntPtr DispatchMessageW(ref MSG msg);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] private static extern bool PostMessageW(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam);
    [DllImport("user32.dll")] private static extern void PostQuitMessage(int code);
    [DllImport("user32.dll")] private static extern bool ShowWindow(IntPtr hwnd, int cmd);
    [DllImport("user32.dll")] private static extern bool IsWindowVisible(IntPtr hwnd);
    [DllImport("user32.dll")] private static extern bool DestroyWindow(IntPtr hwnd);
    [DllImport("user32.dll")] private static extern bool SetWindowPos(IntPtr hwnd, IntPtr after, int x, int y, int cx, int cy, uint flags);
    [DllImport("user32.dll")] private static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint key, byte alpha, uint flags);
    [DllImport("user32.dll")] private static extern int SetWindowRgn(IntPtr hwnd, IntPtr rgn, bool redraw);
    [DllImport("user32.dll")] private static extern bool GetWindowRect(IntPtr hwnd, out RECT rect);
    [DllImport("user32.dll")] private static extern bool GetClientRect(IntPtr hwnd, out RECT rect);
    [DllImport("gdi32.dll")] private static extern IntPtr GetStockObject(int i);
    [DllImport("gdi32.dll")] private static extern bool MoveToEx(IntPtr hdc, int x, int y, IntPtr point);
    [DllImport("gdi32.dll")] private static extern bool LineTo(IntPtr hdc, int x, int y);
    private const int NULL_BRUSH = 5;
    [DllImport("user32.dll")] private static extern bool InvalidateRect(IntPtr hwnd, IntPtr rect, bool erase);
    [DllImport("user32.dll")] private static extern IntPtr BeginPaint(IntPtr hwnd, out PAINTSTRUCT ps);
    [DllImport("user32.dll")] private static extern bool EndPaint(IntPtr hwnd, ref PAINTSTRUCT ps);
    [DllImport("user32.dll")] private static extern int FillRect(IntPtr hdc, ref RECT rect, IntPtr brush);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] private static extern int DrawTextW(IntPtr hdc, string text, int count, ref RECT rect, uint format);
    [DllImport("user32.dll")] private static extern UIntPtr SetTimer(IntPtr hwnd, UIntPtr id, uint ms, IntPtr proc);
    [DllImport("user32.dll")] private static extern IntPtr LoadCursorW(IntPtr instance, IntPtr name);
    [DllImport("user32.dll")] private static extern IntPtr GetDC(IntPtr hwnd);
    [DllImport("user32.dll")] private static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);
    [DllImport("user32.dll")] private static extern bool SetProcessDPIAware();
    [DllImport("user32.dll", EntryPoint = "GetWindowLongPtrW")] private static extern IntPtr GetWindowLongPtrW(IntPtr hwnd, int index);
    [DllImport("user32.dll", EntryPoint = "SetWindowLongPtrW")] private static extern IntPtr SetWindowLongPtrW(IntPtr hwnd, int index, IntPtr value);
    [DllImport("user32.dll")] private static extern IntPtr SetCursor(IntPtr cursor);
    [DllImport("user32.dll")] private static extern bool RegisterHotKey(IntPtr hwnd, int id, uint modifiers, uint vk);
    [DllImport("user32.dll")] private static extern bool UnregisterHotKey(IntPtr hwnd, int id);
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)] private static extern IntPtr GetModuleHandleW(string? name);
    [DllImport("gdi32.dll")] private static extern int GetDeviceCaps(IntPtr hdc, int index);
    [DllImport("gdi32.dll", CharSet = CharSet.Unicode)]
    private static extern IntPtr CreateFontW(int height, int width, int escapement, int orientation, int weight, uint italic, uint underline, uint strikeOut, uint charSet, uint outPrecision, uint clipPrecision, uint quality, uint pitchAndFamily, string face);
    [DllImport("gdi32.dll")] private static extern IntPtr CreateSolidBrush(uint color);
    [DllImport("gdi32.dll")] private static extern IntPtr CreateRoundRectRgn(int x1, int y1, int x2, int y2, int w, int h);
    [DllImport("gdi32.dll")] private static extern IntPtr SelectObject(IntPtr hdc, IntPtr obj);
    [StructLayout(LayoutKind.Sequential)]
    private struct BITMAPINFOHEADER
    {
        public uint biSize;
        public int biWidth, biHeight;
        public ushort biPlanes, biBitCount;
        public uint biCompression, biSizeImage;
        public int biXPelsPerMeter, biYPelsPerMeter;
        public uint biClrUsed, biClrImportant;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MONITORINFO { public uint cbSize; public RECT rcMonitor, rcWork; public uint dwFlags; }

    private const uint MONITOR_DEFAULTTONEAREST = 2;
    [DllImport("user32.dll")] private static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint flags);
    [DllImport("user32.dll")] private static extern bool GetMonitorInfoW(IntPtr monitor, ref MONITORINFO info);
    [DllImport("user32.dll")] private static extern bool UpdateWindow(IntPtr hwnd);
    [DllImport("user32.dll")] private static extern bool PrintWindow(IntPtr hwnd, IntPtr hdc, uint flags);
    [DllImport("gdi32.dll")] private static extern IntPtr CreateCompatibleDC(IntPtr hdc);
    [DllImport("gdi32.dll")] private static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int w, int h);
    [DllImport("gdi32.dll")] private static extern bool DeleteDC(IntPtr hdc);
    [DllImport("gdi32.dll")] private static extern int GetDIBits(IntPtr hdc, IntPtr bmp, uint start, uint lines, byte[] bits, ref BITMAPINFOHEADER bi, uint usage);
    [DllImport("gdi32.dll")] private static extern bool FrameRgn(IntPtr hdc, IntPtr rgn, IntPtr brush, int w, int h);
    [DllImport("gdi32.dll")] private static extern bool DeleteObject(IntPtr obj);
    [DllImport("gdi32.dll")] private static extern int SetBkMode(IntPtr hdc, int mode);
    [DllImport("gdi32.dll")] private static extern uint SetTextColor(IntPtr hdc, uint color);
    [DllImport("gdi32.dll")] private static extern IntPtr CreatePen(int style, int width, uint color);
    [DllImport("gdi32.dll")] private static extern bool Ellipse(IntPtr hdc, int l, int t, int r, int b);
    [DllImport("gdi32.dll")] private static extern bool Polygon(IntPtr hdc, POINT[] points, int count);
    [DllImport("gdi32.dll")] private static extern bool Rectangle(IntPtr hdc, int l, int t, int r, int b);
    [DllImport("gdi32.dll")] private static extern bool RoundRect(IntPtr hdc, int l, int t, int r, int b, int w, int h);
    [DllImport("gdi32.dll")] private static extern bool Chord(IntPtr hdc, int l, int t, int r, int b, int x1, int y1, int x2, int y2);
    [DllImport("user32.dll")] private static extern int FrameRect(IntPtr hdc, ref RECT rect, IntPtr brush);
}
