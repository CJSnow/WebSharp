﻿module.exports = Register;

function Register(pluginPath) {

    
    electron = require('electron')
    const app = electron.app

    const isWindows = process.platform === 'win32';
    const isMac = process.platform === 'darwin';

    var ppapiPath = '';
    if (isWindows) {
        if (process.arch === 'x64')
            ppapiPath = __dirname + '\\..\\bin\\x64\\PepperPlugin.dll';
        else
            ppapiPath = __dirname + '\\..\\bin\\Win32\\PepperPlugin.dll';
    }
    if (isMac) {
        if (process.arch === 'x64')
            ppapiPath = __dirname + '/../bin/mac/libPepperPlugin.so';
        else
            ppapiPath = __dirname + '/../bin/mac/libPepperPlugin.so';
    }
    if (pluginPath)
    {
        if (isWindows) {
            ppapiPath = pluginPath + '\\PepperPlugin.dll';
        }
        if (isMac) {
            ppapiPath = pluginPath + '\\libPepperPlugin.so';
        }
    }
    //console.log('PPAPI path ' + ppapiPath + ';application/electron-dotnet');
    app.commandLine.appendSwitch('register-pepper-plugins', ppapiPath + ';application/electron-dotnet');

}