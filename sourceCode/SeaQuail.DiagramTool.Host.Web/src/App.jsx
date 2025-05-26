import DiagramEditorTool from './tools/DiagramEditorTool';
import DiagramPrintTool from './tools/DiagramPrintTool';

export function App() {
    const path = window.location.hash?.split('#')?.[1]?.toLowerCase().trim('/');
    switch (path) {
        case 'print': return <DiagramPrintTool />;
        default: return <DiagramEditorTool />;
    }
}