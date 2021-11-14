"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.activate = void 0;
const vscode = require("vscode");
const debugger_1 = require("./debugger");
function activate(context) {
    (0, debugger_1.activateDebug)(context, new DebugAdapterFactory());
}
exports.activate = activate;
class DebugAdapterFactory {
    createDebugAdapterDescriptor(session, executable) {
        return new vscode.DebugAdapterExecutable("D:/Documents/Source/BitMagic/BitMagic/bin/Debug/net5.0/BitMagic.exe", ["razor", "compile", "emulate", "--razor-file", session.configuration.program]);
    }
}
//# sourceMappingURL=extension.js.map