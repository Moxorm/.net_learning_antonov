import React, { Component } from 'react';

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
        let contents = this.state.loading
            ? <p><em>Loading... Please refresh once the ASP.NET backend has started. See <a href="https://aka.ms/jspsintegrationreact">https://aka.ms/jspsintegrationreact</a> for more details.</em></p>
            : App.renderForecastsTable(this.state.forecasts);

        return (
            <div>
                <label>
                    Name:
                    <input type="text" id="Name" value={this.state.value} onChange={this.handleChange.bind(this)} default="" />
                </label>
                <button type="button" onClick={this.createTransaction.bind(this)}>
                    Click Me
                </button>
                <h1 id="tabelLabel" >Weather forecast</h1>
                <p>This component demonstrates fetching data from the server.</p>
                {contents}
            </div>
        );
    }

    async createTransaction() {
        //console.log(this.state.Name);

        let TransactionModel = {
            Amount: 1,
            Sender: 13,
            Recipinent: 12,

        };
        const fetchResponse = fetch('pw/CreateTransaction',
            {
                method: "POST",
                headers: { 'Content-type': 'application/json' },
                body: JSON.stringify(TransactionModel)
            });
        console.log(fetchResponse);
        //const data = await response.json();
        //this.setState({ forecasts: data, loading: false });

    }

    async populateWeatherData() {
        const response = await fetch('pw/GetWeatherForecast');
        const data = await response.json();
        this.setState({ forecasts: data, loading: false });
    }


}
