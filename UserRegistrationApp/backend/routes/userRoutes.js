const express = require('express');
const axios = require('axios');
const config = require('../config');
const router = express.Router();

// Get user by ID
router.get('/:userId', async (req, res) => {
    try {
        const response = await axios.get(`${config.baseURL}/${req.params.userId}`);
        res.json(response.data);
    } catch (error) {
        res.status(error.response.status).json({ error: error.response.data });
    }
});

// Insert new user
router.post('/', async (req, res) => {
    try {
        const response = await axios.post(config.baseURL, req.body);
        res.status(201).json(response.data);
    } catch (error) {
        res.status(error.response.status).json({ error: error.response.data });
    }
});

// Update user
router.put('/:userId', async (req, res) => {
    try {
        const response = await axios.put(`${config.baseURL}/${req.params.userId}`, req.body);
        res.json(response.data);
    } catch (error) {
        res.status(error.response.status).json({ error: error.response.data });
    }
});

module.exports = router;

