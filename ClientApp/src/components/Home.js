import React, { Component } from 'react';

export class Home extends Component {
  static displayName = Home.name;

  render () {
    return (
      <div>
        <h1>Welcome</h1>
        <p>Welcome to the Bricket Wood DriveSafe page</p>
        <ul>
          <li>Try the <a href='/vehicle-data'>Vehicle Data</a> page to enter license plates and download a template to fill in with further details</li>
          <li>Coming soon: roadside helper</li>          
        </ul>
        <p>For any help:</p>
        <ul>
          <li>Jamie Cohen: <a href="mailto:jamie@jamiec.co.uk">email me</a></li>
        </ul>
      </div>
    );
  }
}
