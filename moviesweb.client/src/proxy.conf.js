const { env } = require('process');



const PROXY_CONFIG = [
  {
    context: [
      "/api"
    ],
    target: "https://localhost:7088", 
    secure: false,
    changeOrigin: true
  }
]

module.exports = PROXY_CONFIG;
