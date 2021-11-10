import * as vscode from 'vscode';

export class BitMagicAsmEditorProvider implements vscode.CustomTextEditorProvider {

    resolveCustomTextEditor(document: vscode.TextDocument, webviewPanel: vscode.WebviewPanel, token: vscode.CancellationToken): void | Thenable<void> {
        throw new Error('Method not implemented.');    
    }

    public static register(context: vscode.ExtensionContext): vscode.Disposable {
		const provider = new BitMagicAsmEditorProvider(context);
		const providerRegistration = vscode.window.registerCustomEditorProvider(BitMagicAsmEditorProvider.viewType, provider);
		return providerRegistration;
	}

    
	private static readonly viewType = 'bitmagic.csasm';

    constructor(
		private readonly context: vscode.ExtensionContext
	) { }


}