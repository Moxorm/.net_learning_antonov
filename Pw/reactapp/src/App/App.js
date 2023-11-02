import React, { Component } from 'react';
import './App.css';

export default class App extends Component {
    static displayName = App.name;

    constructor(props) {
        super(props);
        this.state = { forecasts: [], loading: true };
    }

    componentDidMount() {
        //this.populateWeatherData();
    }

    static renderForecastsTable(forecasts) {
        return (
            <table className='table table-striped' aria-labelledby="tabelLabel">
                <thead>
                    <tr>
                        <th>Date</th>
                        <th>Temp. (C)</th>
                        <th>Temp. (F)</th>
                        <th>Summary</th>
                    </tr>
                </thead>
                <tbody>
                    {forecasts.map(forecast =>
                        <tr key={forecast.date}>
                            <td>{forecast.date}</td>
                            <td>{forecast.temperatureC}</td>
                            <td>{forecast.temperatureF}</td>
                            <td>{forecast.summary}</td>
                        </tr>
                    )}
                </tbody>
            </table>
        );
    }

    handleChange(event) {
        this.setState({ [event.target.id]: event.target.value });
    }

    render() {
        //let response = this.Userinformation().bind(this)

        return (
            <div>
                <p>
                    Name:
                    <input type="text" id="Name" value={this.state.value} onChange={this.handleChange.bind(this)} default="" />
                </p>
                <p>
                    Password:
                    <input type="text" id="Password" value={this.state.value} onChange={this.handleChange.bind(this)} default="" />
                </p>
                <button type="button" onClick={this.login.bind(this)}>
                    Login
                </button>
                <button type="button" onClick={this.createTransaction.bind(this)}>
                    transaction
                </button>

            </div>
        );
    }
    async login() {

        const fetchResponse = fetch('account/Login?email=' + this.state.Name + '&password=' + this.state.Password,
            {
                method: "POST",
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json',
                    'Authorization': 'Basic ' + btoa(this.state.Name + ':' + this.state.Password)
                }
            });
        console.log(fetchResponse);
    }
    async logout() {

        const fetchResponse = fetch('account/Logout',
            {
                method: "POST",
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json',
                }
            });
        console.log(fetchResponse);
    }
    async Userinformation() {

        const fetchResponse = fetch('account/Userinformation',
            {
                method: "Get",
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                }
            });
        return fetchResponse;
    }
    async createTransaction() {

        //let TransactionModel = {
        //    Amount: 1,
        //    Sender: 13,
        //    Recipinent: 12,
       // };
        const fetchResponse = fetch('pw/CreateTransaction?amount=' + 1/*this.state.Amount*/ + '&recipinent=' + 19,//this.state.Recipinent,
            {
                method: "POST",
                headers: { 'Content-type': 'application/json' },
                //body: JSON.stringify(TransactionModel)
            });
        console.log(fetchResponse);
        //const data = await response.json();
        //this.setState({ forecasts: data, loading: false });

    }


}
