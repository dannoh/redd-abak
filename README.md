# redd-abak
A simple tool for exporting Abak timesheet entries.
![Alt text](Docs/Screenshot.png?raw=true "Sample Screenshot")
## WPF Application
There is a WPF application that can extract the entries as raw JSON, or export it to Xero.  The app works, though might be a little rough around the edges.
#### Plugins
You can create your own export targets, there are examples in the JsonExportService, or JsonViewExportService.

- Add AbakHelper.Integration as a reference to your project
- Mark the reference as Copy: False https://stackoverflow.com/a/32885170/807079


## Electron
For anyone looking for some cross-platform support, there is also an Electron app I was playing with.  It also has an example export for JSON download to show how to wire it up.
