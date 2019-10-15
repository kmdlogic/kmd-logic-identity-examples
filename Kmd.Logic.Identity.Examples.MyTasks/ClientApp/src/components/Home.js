import React, { Component } from 'react';

export class Home extends Component {
  static displayName = Home.name;

  render () {
    return (
      <div>
        <h1>Overview</h1>
        <p>This is the My Tasks Single Page Application (SPA). Use it to:</p>
        <ul>
          <li>Manage your list of TODO items</li>
          <li>View upcoming dates of interest</li>
        </ul>
        <p>To get started you first need to login!</p>
      </div>
    );
  }
}
