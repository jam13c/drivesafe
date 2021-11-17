import React, { Component } from 'react';
import { LicensePlateInput } from './widgets/LicensePlateInput'
import { VehicleTable } from './widgets/VehicleTable'
import { saveAs } from 'file-saver'

export class VehicleData extends Component {
    
    constructor(props) {
        super(props);

        this.state = {
            tableVisible: false,
            vehicleData:[]
        }
        this.handleView = this.handleView.bind(this);
        this.handleDownload = this.handleDownload.bind(this);
    }

    async handleView(registrationNumbers) {
        const response = await fetch(`${window.location.origin}/vehicles`, {
            method: "POST",
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ registrationNumbers })
        });

        const vehicleData = await response.json();
        this.setState({
            tableVisible: true,
            vehicleData
        })
        
    }
    async handleDownload(registrationNumbers) {
        const response = await fetch(`${window.location.origin}/vehicles/download`, {
            method: "POST",
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ registrationNumbers })
        });
        const file = await response.blob();
        saveAs(file, "driversafe.csv");
    }


    render() {
        return (
            <div class="container">
                <div class="row">
                    <div class="col">
                        <h1>Vehicle Data</h1>
                        <p>
                            Enter a list of vehicle license plates in the text area below, separating each one with a new line.
                            You can then click <strong>View</strong> to see the DVLA details on these vehicles or
                            click <strong>Download</strong> to get a formatted CSV file ready to fill in the additional details needed
                        </p>
                    </div>
                </div>
                <div class="row">
                    <div class="col-sm-4">
                        <LicensePlateInput view={this.handleView} download={this.handleDownload} />
                    </div>
                    <div class="col">
                        {this.state.tableVisible
                            ? <VehicleTable visible={this.state.visible} vehicles={this.state.vehicleData} />
                            : <div class="h-100 w-100 align-middle">Enter registration numbers in the box to the left and click <strong>View</strong> to see details here</div>
                        }
                    </div>
                </div>
            </div>
        );
    }
}