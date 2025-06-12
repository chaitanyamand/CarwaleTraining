const express = require("express")
const app = express();
const cors = require("cors");

app.use(cors());
app.get("/todos",(req,res) => {
    return res.json([
        {id: "siusisjsksowqs", text: "Todo1"},
        {id: "sjqoisjskjiwss", text: "Todo2"},
        {id: "kwiwuiyrteuiiw", text: "Todo3"},
        {id: "kwhikxxjiwdimx", text: "Todo4"},
        {id: "sjnxsiudhiudjs", text: "Todo5"},
    ])
})

app.listen(8080, () => {
    console.log("Server listening on port 8080")
})