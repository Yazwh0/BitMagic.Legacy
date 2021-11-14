import * as vscode from 'vscode'

export function activateDebug(context: vscode.ExtensionContext, factory: vscode.DebugAdapterDescriptorFactory)
{
    console.log("activate")

    context.subscriptions.push(
        vscode.commands.registerCommand('extension.csasm-debug.runEditorContents', (resource: vscode.Uri) => {
            console.log("run editor");
            let targetResource = resource;
			if (!targetResource && vscode.window.activeTextEditor) {
				targetResource = vscode.window.activeTextEditor.document.uri;
			}
			if (targetResource) {
				vscode.debug.startDebugging(undefined, {
					type: 'csasm',
					name: 'Run File',
					request: 'launch',
					program: targetResource.fsPath
				},
					{ noDebug: true }
				);
			}
        })
    );

    context.subscriptions.push(
        vscode.commands.registerCommand('extension.csasm-debug.debugEditorContents', (resource: vscode.Uri) => {
            console.log("debug editor");
            let targetResource = resource;
			if (!targetResource && vscode.window.activeTextEditor) {
				targetResource = vscode.window.activeTextEditor.document.uri;
			}
			if (targetResource) {
				vscode.debug.startDebugging(undefined, {
					type: 'csasm',
					name: 'Debug File',
					request: 'launch',
					program: targetResource.fsPath,
					stopOnEntry: true
				});
			}
        })
    );

    context.subscriptions.push(vscode.debug.registerDebugAdapterDescriptorFactory('csasm', factory));

}