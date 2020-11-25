const { dialog }= require('electron')
const path = require('path'); 
const fs = require('fs'); 

class JsonDownload {
    name = "Json Download";
    execute = (items) => {
        console.log(items);
        dialog.showSaveDialog({
            title: 'Select the File Path to save',
            defaultPath: path.join(__dirname, 'export.json'),
            buttonLabel: 'Save',
            properties: []
        }).then(file => {
            // Stating whether dialog operation was cancelled or not. 
            if (!file.canceled) {
                fs.writeFile(file.filePath.toString(),
                    JSON.stringify(items), function (err) {
                        if (err) throw err;
                    });
            }
        }).catch(err => {
            console.log(err)
        });
    };
}
module.exports = new JsonDownload();
