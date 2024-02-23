/** @type {import('next').NextConfig} */
const nextConfig = {
    images: {
        remotePatterns: [
            {
                hostname: "cdn.pixabay.com"
            }    
        ]
    },
    output: 'standalone'
}

module.exports = nextConfig
