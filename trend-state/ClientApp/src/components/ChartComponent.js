import React from 'react';
import { render } from 'react-dom';
import Chart from './Chart';
import { getData } from "./utils"

import { TypeChooser } from "react-stockcharts/lib/helper";

export class ChartComponent extends React.Component {
    componentDidMount() {
        //getData().then(data => {
        //    this.setState({ data })
        //})

        fetch('api/SampleData/Candles')
            .then(response => response.json())
            .then(candles => candles.map(c => {
                c.date = new Date(c.date);
                return c;
            }))
            .then(data => {
                this.setState({ data });
            });
    }

    render() {
        if (this.state == null) {
            return <div>Loading...</div>
        }
        return (
            <div>
                <Chart type="hybrid" data={this.state.data} />
            </div>
        )
    }
}
