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
            webApiService(webApiConfig.apiURI, response.accessToken, (response) => {
                this.setState({profile: response});
            }).catch(err => console.log(err));
        });
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
                                        <ListGroup.Item>Employee Id: {this.state.profile.employeeId}</ListGroup.Item>
                                        <ListGroup.Item>Department: {this.state.profile.department}</ListGroup.Item>
                                        <ListGroup.Item>Display Name: {this.state.profile.displayName}</ListGroup.Item>
                                        <ListGroup.Item>Given Name: {this.state.profile.givenName}</ListGroup.Item>
                                        <ListGroup.Item>Country: {this.state.profile.country}</ListGroup.Item>
                                        <ListGroup.Item>City: {this.state.profile.city}</ListGroup.Item>
                                    </ListGroup>      
                                : null
                            }
                        </Card.Body>
                    </Card>
                </div>
            </div>
        );
    }
}

App.propTypes = {
    account: PropTypes.object,
    error: PropTypes.string,
    isAuthenticated: PropTypes.bool,
    signIn: PropTypes.func.isRequired,
    signOut: PropTypes.func.isRequired,
    acquireToken: PropTypes.func.isRequired,
}
export default AuthProvider(App);
