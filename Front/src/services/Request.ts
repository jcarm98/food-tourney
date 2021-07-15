export default class Request {
    // Simple HTTP request wrapper for get and post
    request(
        method: string,
        url: string,
        body: string,
        callback: any
    ): void {
        const HTTP = new XMLHttpRequest();
        HTTP.open(method, url, true);
        HTTP.withCredentials = false;
        if (method == "GET") HTTP.send();
        else if (method == "POST") HTTP.send(body);

        HTTP.onreadystatechange = function () {
            if (this.readyState === 4 && this.status === 200) {
                callback(HTTP.responseText);
            }
        };
    }

/*
function request( method, url, body, callback) {
    const HTTP = new XMLHttpRequest();
    HTTP.open(method, url, true);
    HTTP.withCredentials = false;
    if (method == "GET")
        HTTP.send();
    else if (method == "POST")
        HTTP.send(body);

    HTTP.onreadystatechange = function () {
        if (this.readyState === 4 && this.status === 200) {
            callback(HTTP.responseText);
        }
    };
}
var url = 'https://foodtourney.app:5000/popular/1111?qs=321&aws=abc';
request("GET", url, JSON.stringify({Lat: 1.1, Lng: -1.1, Name: "Test" }), (response) =>{
              console.log(JSON.parse(response));
             });
*/
}