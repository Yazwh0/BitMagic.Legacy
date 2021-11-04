import * as vscode from 'vscode';
import { BitMagicAsmEditorProvider } from './bitMagicAsmEditor';

export function activate(context: vscode.ExtensionContext) {
	// Register our custom editor providers
	context.subscriptions.push(BitMagicAsmEditorProvider.register(context));
}