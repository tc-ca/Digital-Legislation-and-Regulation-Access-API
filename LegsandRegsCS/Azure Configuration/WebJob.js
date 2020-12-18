
//This code should be added to an Azure WebJob under the App Service
//Use the cron string "0 0 5 * * *" to run the job at 5:00 UTC which is midnight EST
//This WebJob is meant for triggering a nightly update that checks the primary data source

const https = require('https');

const options = {
    hostname: '<<hostname>>',//Add your host name
    port: 443,
    path: '/api/update',//Confirm the path
    method: 'GET',
    headers: {
        'password': '<<password>>'//Add password
    }

}

const req = https.request(options, res => {
    console.log(`statusCode: ${res.statusCode}`)

    res.on('data', d => {
        process.stdout.write(d)
    })
})

req.on('error', error => {
    console.error(error)
})

req.end()