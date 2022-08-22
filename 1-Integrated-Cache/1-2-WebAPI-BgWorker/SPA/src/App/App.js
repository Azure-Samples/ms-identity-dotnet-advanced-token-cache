import React from "react";
import PropTypes from "prop-types";
import AuthProvider  from "../utils/authProvider";
import { webApiService }  from "../services/webApiService";
import { webApiConfig, tokenRequest, loginRequest } from '../utils/authConfig';
import { 
    Nav, 
    Navbar, 
    Button,
    Card,
    ListGroup
} from 'react-bootstrap';

import 'bootstrap/dist/css/bootstrap.min.css';
import './App.css';

class App extends React.Component {

    constructor(props) {
        super(props)

        this.state = {
            profile: null,
        }
    }

    callWebApi = () => {
        this.props.acquireToken(tokenRequest).then((response) => {
            if (response.accessToken) {
                webApiService(webApiConfig.apiURI, response.accessToken, (response) => {
                    this.setState({profile: response});
                });
            }
        }).catch(err => console.log(err));
    }
    
    handleSignIn = () => {
        this.props.signIn();
    }

    handleSignOut = () => {
        this.props.signOut();
    }

    render() {
        return (
            <div className="app">
                <Navbar className="navbar" bg="dark" variant="dark">
                    <Navbar.Brand href="/">Microsoft Identity Platform</Navbar.Brand>
                    <Nav className="mr-auto">
                    </Nav>
                    {
                        this.props.isAuthenticated ? 
                        <Button variant="info" onClick={this.handleSignOut}>Logout</Button> 
                        : 
                        <Button variant="outline-info" onClick={this.handleSignIn}>Login</Button>
                    }
                </Navbar>
                <div>
                {
                    this.props.account ?
                    <Card>
                        <Card.Header>Welcome {this.props.account ? this.props.account.username : "User"}</Card.Header>
                        <Card.Body>
                            <Card.Title>User Profile</Card.Title>
                            <Card.Text>
                                Call the API to obtain your profile.
                            </Card.Text>
                            <Button variant="outline-info" onClick={this.callWebApi}>Call Web API</Button>
                            {
                                this.state.profile ?
                                    <ListGroup variant="flush">
                                        <ListGroup.Item>Employee Id: {this.state.profile.EmployeeId}</ListGroup.Item>
                                        <ListGroup.Item>Department: {this.state.profile.Department}</ListGroup.Item>
                                        <ListGroup.Item>Display Name: {this.state.profile.DisplayName}</ListGroup.Item>
                                        <ListGroup.Item>Given Name: {this.state.profile.GivenName}</ListGroup.Item>
                                        <ListGroup.Item>Country: {this.state.profile.Country}</ListGroup.Item>
                                        <ListGroup.Item>City: {this.state.profile.City}</ListGroup.Item>
                                    </ListGroup>      
                                : null
                            }
                        </Card.Body>
                    </Card> : null
                }
                </div>
            </div>
        );
    }
}

export default AuthProvider(App);
