<div align="center">

[English](README.md) · [Français](README.fr.md) · [Español](README.es.md) · **Português**

<img src="docs/icon.png" alt="" width="88">

# Contador de Baús do Fortnite

**Conta cada baú que você abre no Fortnite, partida após partida, com um overlay que se mistura ao jogo.**

[**⬇ Baixar ChestCounter.exe**](../../releases/latest) · Windows 10 / 11 · grátis · sem instalação

</div>

---

> Esta é uma versão personalizada do [Fortnite Chest Counter](https://github.com/LED-N/Fortnite-ChestCounter) de LED-N, com:
> - **interface em português (Brasil)**;
> - **temas de cores** (Pirata, Brasil, Neon, Gelo, Lava) para a janela, o relatório e o overlay;
> - uma seção **Suas estatísticas** no relatório (baús por hora, sequência de dias, semana atual × anterior, fase recente, por modo e por colocação);
> - **metas** diárias e semanais no overlay, **21 conquistas**, **ranking do esquadrão**, **ritmo na partida**, **baús × resultado** e o **loot** dos seus baús;
> - **mapa dos seus baús** no relatório (mapa e locais baixados de fortnite-api.com, atualizados toda semana);
> - **modo desempenho** para PCs mais fracos (sem leitura durante a partida, prioridade mínima, relatório enxuto);
> - resumo opcional das partidas num canal do **Discord** (via webhook; desligado por padrão, e nada além do resumo é enviado).
>
> As atualizações do exe vêm deste repositório; os dados da caça ao tesouro continuam vindo do projeto original. Para todo o resto (funcionamento, segurança, FAQ), veja o [README em inglês](README.md).

## Como funciona

O contador lê os **replays** que o Fortnite salva no seu PC (`%LOCALAPPDATA%\FortniteGame\Saved\Demos`) depois de cada partida e soma os baús que **você** abriu. Ele não modifica o jogo, não acessa a memória do Fortnite e não envia nenhum dado.

> [!IMPORTANT]
> **Ative os replays no Fortnite**: *Configurações → Jogo → Replays → Gravar Replays: Ativado*. Sem replays, a ferramenta não tem o que ler.

1. Baixe o [`ChestCounter.exe`](../../releases/latest) e execute (o Windows pode avisar que o exe não é assinado: *Mais informações → Executar assim mesmo*).
2. Em **Configurações**, escolha **Português (Brasil)** e o **Tema** que preferir.
3. Deixe o contador aberto enquanto joga: os baús de cada partida entram alguns segundos depois que ela termina.

## Overlay

| Atalho | O que faz |
|---|---|
| **Ctrl+Shift+O** | Mostra / esconde o overlay (funciona dentro do jogo). |
| **Ctrl+Shift+L** | Trava / destrava (destravado: arraste para mover). |

Use o Fortnite em **Tela Cheia em Janela** para o overlay aparecer por cima do jogo.

## Compilar

Instale o [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) e dê dois cliques em **`build.bat`**: o exe sai em `dist\ChestCounter.exe`. Para publicar uma versão, crie uma tag `v*` (ex.: `v1.6.0`) e dê push: o GitHub Actions compila e anexa o exe ao release, e os contadores instalados oferecem a atualização.

### Puxar novidades do projeto original

O histórico deste fork foi reescrito, então ele não compartilha commits com o original e um `git merge` não funciona de forma limpa. Traga as novidades com `cherry-pick`, a partir do último commit do original já incluído aqui (`a2c309b`, "Merge pull request #59"):

```
git fetch upstream
git cherry-pick --no-merges a2c309b..upstream/main
```

Depois de sincronizar, anote o novo último commit do original para usar na próxima vez.

Textos novos do original sem tradução aparecem em inglês até receberem o 4º argumento (`L.T("fr", "en", "es", "pt")`).

## Licença

[MIT](LICENSE), como o projeto original. Este projeto não é afiliado, endossado ou patrocinado pela Epic Games. Fortnite é uma marca registrada da Epic Games, Inc.
