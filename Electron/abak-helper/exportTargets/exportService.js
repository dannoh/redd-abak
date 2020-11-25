class ExportService {
    constructor() {
        console.log("created an export service", new Date());
    }
    items = [];
    setItems = (items) => {
        console.log("setItems", items);
        this.items = items;
    }

    getItems = () => {
        return this.items;
    }
}


module.exports = new ExportService();