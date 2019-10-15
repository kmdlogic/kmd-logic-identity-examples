import React, { Component } from 'react';
import axios from 'axios';
import { Col, Button, Form, FormGroup, Label, Input, Alert } from 'reactstrap';
import DatePicker from 'react-datepicker';
import "react-datepicker/dist/react-datepicker.css";
import authService from './api-authorization/AuthorizeService';

const formStyle = {
  margin: '0px 0px 20px 0px',
  padding: '10px',
  border: '2px solid LightGray'
};

export class MyTasks extends Component {
  static displayName = MyTasks.name;

  constructor (props) {
    super(props);
    this.onFormSubmit = this.onFormSubmit.bind(this);
    this.state = { 
      todos: [], 
      loading: true,
      todoDate: null,
      todoDesc: '',
      errorMsg: undefined
    };

    this.refreshTodos();
  }

  async refreshTodos() {
    const user = await authService.getUser();
    const token = await authService.getAccessToken();

    console.log(token);

    var config = {
      headers: { Authorization: "bearer " + token }
    };
    axios.get(`${window.appConfig.TodosApiUrl}api/todos/${user.sub}`, config)
      .then(res => this.setState({ todos: res.data, loading: false }))
      .catch(error => {
        this.setState({ todos: [], loading: false, errorMsg: error.description });
      });
  }

  async onFormSubmit(event) {
    event.preventDefault();

    if (!this.state.todoDate || !this.state.todoDesc) {
      alert("Date and description are required");
    } else {
      const postContent = {
        date: this.state.todoDate.toDateString(),
        description: this.state.todoDesc
      };

      const user = await authService.getUser();

      const token = await authService.getAccessToken();
      var config = {
        headers: { Authorization: "bearer " + token }
      };

      axios.post(`${window.appConfig.TodosApiUrl}api/todos/${user.sub}`, postContent, config)
        .then(res => {
          this.setState({ todoDate: null, todoDesc: '' });
          this.refreshTodos()
        })
        .catch(error => {
          this.setState({ loading: false, errorMsg: error.description });
        });
    }
  }

  renderTodoInput () {
    return (
      <Form style={formStyle} onSubmit={this.onFormSubmit}>
        <FormGroup row>
          <Label for="todo-date" sm={12}>Date</Label>
          <Col sm={12}>
            <DatePicker name="date" id="todo-date" autoComplete='off' dateFormat="dd/MM/yyyy" selected={this.state.todoDate} onChange={d => this.setState({ todoDate: d })} />
          </Col>
        </FormGroup>
        <FormGroup>
          <Label for="todo-description">Description</Label>
          <Input type="textarea" name="text" id="todo-description" value={this.state.todoDesc} onChange={e => this.setState({ todoDesc: e.target.value })} />
        </FormGroup>
        <Button type="submit">Add</Button>
      </Form>
    );
  }

  static renderTodosTable (todos) {
    return (
      <table className='table table-striped'>
        <thead>
          <tr>
            <th>Date</th>
            <th>Description</th>
          </tr>
        </thead>
        <tbody>
          {todos.map(todo =>
            <tr key={todo.date}>
              <td>{new Date(todo.date).toLocaleDateString()}</td>
              <td>{todo.description}</td>
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
      : MyTasks.renderTodosTable(this.state.todos);

    let errorMessage = MyTasks.renderErrorMessage(this.state.errorMsg);

    return (
      <div>
        <h1>TODO</h1>
        {errorMessage}
        {this.renderTodoInput()}
        <p>Your list of outstanding tasks to complete.</p>
        {contents}
      </div>
    );
  }
}
