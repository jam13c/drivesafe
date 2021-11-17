import React, { Component } from 'react';

export class LicensePlateInput extends Component {
    constructor(props) {
        super(props);
        this.state = {
            value: ''
        };

        this.handleChange = this.handleChange.bind(this);
        this.handleView = this.handleView.bind(this);
        this.handleDownload = this.handleDownload.bind(this);
    }

    handleChange(event) {
        this.setState({ value: event.target.value });
    }

    handleView(event) {
        this.props?.view(this.state.value.trim("\n").split("\n"));
        event.preventDefault();
    }

    handleDownload(event) {
        this.props?.download(this.state.value.trim("\n").split("\n"));
        event.preventDefault();
    }

    render() {
        return (
            <>                 
                <textarea rows="20" value={this.state.value} onChange={this.handleChange} />
                <div>
                    <button type="button" className="btn btn-large btn-primary" onClick={this.handleView}>View</button>
                    <button type="button" className="btn btn-large btn-primary" onClick={this.handleDownload}>Download</button>
                </div>
            </>
           
        );
    }
}