"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.activateDebug = void 0;
const vscode = require("vscode");
function activateDebug(context, factory) {
    console.log("activate");
    context.subscriptions.push(vscode.commands.registerCommand('extension.csasm-debug.runEditorContents', (resource) => {
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
            }, { noDebug: true });
        }
    }));
    context.subscriptions.push(vscode.commands.registerCommand('extension.csasm-debug.debugEditorContents', (resource) => {
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
    }));
    context.subscriptions.push(vscode.debug.registerDebugAdapterDescriptorFactory('csasm', factory));
}
exports.activateDebug = activateDebug;
//# sourceMappingURL=debugger.js.map