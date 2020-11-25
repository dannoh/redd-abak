// All of the Node.js APIs are available in the preload process.
const {ipcRenderer} = require('electron');

// It has the same sandbox as a Chrome extension.
window.addEventListener('DOMContentLoaded', () => {
  const replaceText = (selector, text) => {
    const element = document.getElementById(selector)
    if (element) element.innerText = text
  }

  for (const type of ['chrome', 'node', 'electron']) {
    replaceText(`${type}-version`, process.versions[type])
  }
})

const onTimesheetDownloaded = function(data) {
  data = data.replaceAll(/(new Date\(.*?\))/g, "\"$1\"");
  var result = JSON.parse(data, (key, value) => {
    if(key === "Date") {
      return eval(value);
    } else { return value; }
  });
  ipcRenderer.send("onTimesheetDownloaded", result);
}

window.onTimesheetDownloaded = onTimesheetDownloaded;