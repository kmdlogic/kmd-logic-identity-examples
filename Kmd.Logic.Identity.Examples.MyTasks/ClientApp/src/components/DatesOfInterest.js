import React, { Component } from 'react';
import { Alert } from 'reactstrap';
import axios from 'axios';
import authService from './api-authorization/AuthorizeService';

export class DatesOfInterest extends Component {
  static displayName = DatesOfInterest.name;

  constructor (props) {
    super(props);
    this.state = { 
      datesOfInterest: [], 
      loading: true,
      errorMsg: undefined 
    };

    this.refreshDates();
  }

  async refreshDates() {
    const token = await authService.getAccessToken();

    var config = {
      headers: { Authorization: "bearer " + token }
    };
    axios.get(`${window.appConfig.DatesApiUrl}api/dates`, config)
      .then(res => this.setState({ datesOfInterest: res.data, loading: false, errorMsg: undefined }))
      .catch(error => {
        this.setState({ datesOfInterest: [], loading: false, errorMsg: error.description });
      });
  }

  static renderDatesTable (datesOfInterest) {
    return (
      <table className='table table-striped'>
        <thead>
          <tr>
            <th>Date</th>
            <th>Description</th>
          </tr>
        </thead>
        <tbody>
          {datesOfInterest.map(dateOfInterest =>
            <tr key={dateOfInterest.date}>
              <td>{new Date(dateOfInterest.date).toLocaleDateString()}</td>
              <td>{dateOfInterest.description}</td>
            </tr>
          )}
        </tbody>
      </table>
    );
  }

  static renderErrorMessage (errorMessage) {
    if (errorMessage) {
      return (
        <div>
          <Alert color="danger">
            {errorMessage}
          </Alert>
        </div>
      );
    } 
  }

  render () {
    let contents = this.state.loading
      ? <p><em>Loading...</em></p>
      : DatesOfInterest.renderDatesTable(this.state.datesOfInterest);

    let errorMessage = DatesOfInterest.renderErrorMessage(this.state.errorMsg);

    return (
      <div>
        <h1>Dates of Interest</h1>
        {errorMessage}
        <p>The following dates of interest have been centrally published and made available to you for reference.</p>
        {contents}
      </div>
    );
  }
}
