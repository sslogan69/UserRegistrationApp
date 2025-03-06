import React, { useState } from 'react';
import axios from 'axios';
import '../styles/UserForm.css';

function UserForm() {
    const [user, setUser] = useState({ userId: '', userName: '', email: '', passwordHash: '' });
    const [message, setMessage] = useState('');

    const handleChange = (e) => {
        const { name, value } = e.target;
        setUser({ ...user, [name]: value });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            const response = await axios.post('https://localhost:7043/api/User', user);
            setMessage('User registered successfully');
        } catch (error) {
            setMessage('Error registering user');
        }
    };

    return (
        <div className="user-form-container">
            <form className="user-form" onSubmit={handleSubmit}>
                <h2>Register User</h2>
                <div className="form-group">
                    <label>User Name:</label>
                    <input type="text" name="userName" value={user.userName} onChange={handleChange} required />
                </div>
                <div className="form-group">
                    <label>Email:</label>
                    <input type="email" name="email" value={user.email} onChange={handleChange} required />
                </div>
                <div className="form-group">
                    <label>Password:</label>
                    <input type="password" name="passwordHash" value={user.passwordHash} onChange={handleChange} required />
                </div>
                <button type="submit" className="submit-button">Register</button>
            </form>
            {message && <p className="message">{message}</p>}
        </div>
    );
}

export default UserForm;


