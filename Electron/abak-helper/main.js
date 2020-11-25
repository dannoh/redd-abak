// Modules to control application life and create native browser window
const { app, BrowserWindow, Menu, protocol, session, net, ipcMain } = require('electron')
const path = require('path');
const settings = require('electron-settings');
const exportService = require('./exportTargets/exportService');
const JsonDownload = require('./exportTargets/jsonDownload');
const exportTargets = [JsonDownload];

console.log("url");
ipcMain.on("onTimesheetDownloaded", (event, args) => {
  exportService.setItems(args);
});

async function createWindow() {
  // Create the browser window.
  const url = await settings.get("url");
  const mainWindow = new BrowserWindow({
    width: 1200,
    height: 800,
    webPreferences: {
      preload: path.join(__dirname, 'preload.js'),
      nodeIntegration: true
    }
  })
  mainWindow.maximize();
  var menu = Menu.buildFromTemplate([
    {
      label: 'File',
      submenu: [
        { label: 'Exit' }
      ]
    },

    {
      label: 'Export',
      submenu: exportTargets.map(c => ({ label: c.name, click: () => c.execute(exportService.getItems()) }))
    }
  ])
  Menu.setApplicationMenu(menu);

  // and load the index.html of the app.
  //mainWindow.loadFile('index.html')
  if (url) {
    loadAbak(url, mainWindow);
  }
  else {
    mainWindow.loadFile("settings.html");
    // const child = new BrowserWindow({ parent: mainWindow, modal: true, show: false })    
    // child.loadFile("settings.html");
    // child.once('ready-to-show', () => {
    //   child.show()
    // })
  }

  // Modify the user agent for all requests to the following urls.
  // Open the DevTools.
  mainWindow.webContents.openDevTools()
}

loadAbak = (url, window) => {
  const filter = {
    urls: [`${url}/Abak/Transact/GetGroupedTransacts/*`]
  }

  session.defaultSession.webRequest.onCompleted(filter, (details) => {
    const query = details.uploadData[0].bytes.toString("utf8");
    const params = Object.fromEntries(new URLSearchParams(query));
    var postData = JSON.stringify(params);
    //the 0; at the end is important, otherwise you get some cloning errors
    var js = `Ext.Ajax.request({url: \"${url}/Abak/Transact/GetGroupedTransacts?ignore\",params: ${postData},dataType: \"json\",type: \"POST\",success: function(result) {window.onTimesheetDownloaded(result.responseText);}});0;`;
    window.webContents.executeJavaScript(js);

  });

  window.loadURL(url);

}
ipcMain.on("save-settings", async (event, newSettings) => {
  debugger;
  //const urlChanged = settings.get("url") !== newSettings.url;
  await settings.set(newSettings)
  loadAbak(newSettings.url, BrowserWindow.mainWindow);
  // const window = BrowserWindow.getAllWindows().find(c => c.isModal());
  // window.webContents.openDevTools();
  // window.close();
  // if (urlChanged)
  //   BrowserWindow.mainWindow.loadURL(newSettings.url);
});

// This method will be called when Electron has finished
// initialization and is ready to create browser windows.
// Some APIs can only be used after this event occurs.
app.whenReady().then(() => {
  createWindow()

  app.on('activate', function () {
    // On macOS it's common to re-create a window in the app when the
    // dock icon is clicked and there are no other windows open.
    if (BrowserWindow.getAllWindows().length === 0) createWindow()
  })
})

// Quit when all windows are closed, except on macOS. There, it's common
// for applications and their menu bar to stay active until the user quits
// explicitly with Cmd + Q.
app.on('window-all-closed', function () {
  if (process.platform !== 'darwin') app.quit()
})

// In this file you can include the rest of your app's specific main process
// code. You can also put them in separate files and require them here.
