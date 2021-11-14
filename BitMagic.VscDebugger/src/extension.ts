import * as vscode from 'vscode';
import { activateDebug } from './debugger';


export function activate(context: vscode.ExtensionContext) {
    activateDebug(context, new DebugAdapterFactory());
}

class DebugAdapterFactory implements vscode.DebugAdapterDescriptorFactory {

    createDebugAdapterDescriptor(session: vscode.DebugSession, executable: vscode.DebugAdapterExecutable | undefined): vscode.ProviderResult<vscode.DebugAdapterDescriptor> {
        return new vscode.DebugAdapterExecutable("D:/Documents/Source/BitMagic/BitMagic/bin/Debug/net5.0/BitMagic.exe", ["razor", "compile", "emulate", "--razor-file", session.configuration.program] );
    }
}