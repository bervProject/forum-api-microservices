import express from 'express';

const port = parseInt(process.env.PORT || '9000');
const app = express();
app.use(express.json());

app.get("/", (req, res) => {
    res.json({
        message: 'Hello World!',
    });
})

app.listen(port, () => {
    console.log(`Server listen on port ${port}`)
});