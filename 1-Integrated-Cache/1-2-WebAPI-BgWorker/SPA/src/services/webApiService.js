// Helper function to call the API endpoint 
// using authorization bearer token scheme
export const webApiService = async(endpoint, token, callback) => {

    const headers = new Headers();
    const bearer = `Bearer ${token}`;

    headers.append("Authorization", bearer);

    const options = {
        method: "GET",
        headers: headers
    };

    console.log('request made to API at: ' + new Date().toString());
    
    fetch(endpoint, options)
        .then(response => response.json())
        .then(response => {
            console.log(response);
            callback(response);
        })
        .catch(error => console.log(error));
}