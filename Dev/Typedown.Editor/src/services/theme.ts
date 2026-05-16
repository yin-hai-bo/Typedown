import transport from 'services/transport';
import { remote } from 'services/remote';

transport.addListener('ThemeChanged', onThemeChanged)
transport.addListener<Record<string, unknown>>('SettingsChanged', onSettingsChanged)

remote.getCurrentTheme().then(arg => {
    onThemeChanged(arg);
    setTimeout(() => remote.contentLoaded(), 0);
})

function getorCreateStyle(id: string) {
    let style = document.getElementById(id) as HTMLLinkElement;
    if (!style) {
        style = document.createElement("link");
        style.rel = "stylesheet";
        style.id = id;
        document.head.appendChild(style)
    }
    return style;
}

function normalizeDocumentTheme(theme: unknown) {
    if (typeof theme === 'string' && theme.length > 0) {
        return theme.toLowerCase()
    }
    if (theme === 1) {
        return 'minimal'
    }
    if (theme === 2) {
        return 'paper'
    }
    return 'github'
}

function updateDocumentTheme(theme: unknown) {
    const documentStyleDocument = getorCreateStyle("link_style_document");
    const documentTheme = normalizeDocumentTheme(theme)
    documentStyleDocument.href = `theme/document/${documentTheme}.theme.css`
    document.documentElement.style.setProperty('--documentTheme', documentTheme)
    ; (window as any).documentTheme = documentTheme
}

function onThemeChanged({ theme, accentColor, background, documentTheme }: any) {
    const editorStyleDocument = getorCreateStyle("link_style_editor");
    const prismjsStyleDocument = getorCreateStyle("link_style_prismjs");
    const codemirrorStyleDocument = getorCreateStyle("link_style_codemirror");

    theme = theme?.toLowerCase()
    editorStyleDocument.href = `theme/editor/${theme}.theme.css`
    prismjsStyleDocument.href = `theme/prismjs/${theme}.theme.css`
    codemirrorStyleDocument.href = `theme/codemirror/${theme}.theme.css`

    const themeColorAlphas = [10, 20, 30, 40, 50, 60, 70, 80, 90]
    const { r, g, b, a } = accentColor
    const { R: bgR, G: bgG, B: bgB, A: bgA } = background

    document.body.style.backgroundColor = `rgba(${bgR}, ${bgG}, ${bgB}, ${bgA})`;
    document.documentElement.style.setProperty('--actualTheme', theme)
    document.documentElement.style.setProperty('--themeColor', `rgba(${r}, ${g}, ${b}, ${a})`)
    themeColorAlphas.forEach(e => document.documentElement.style.setProperty(`--themeColor${e}`, `rgba(${r}, ${g}, ${b}, ${a * (e / 100)})`))
    updateDocumentTheme(documentTheme)

    ; (window as any).actualTheme = theme
}

function onSettingsChanged(newOptions: Record<string, unknown>) {
    if ('DocumentTheme' in newOptions || 'documentTheme' in newOptions) {
        updateDocumentTheme(newOptions.DocumentTheme ?? newOptions.documentTheme)
    }
}
