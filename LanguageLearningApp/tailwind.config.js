module.exports = {
    content: [
        './Pages/**/*.cshtml',
        './Views/**/*.cshtml'
    ],
    theme: {
        extend: {
            colors: {
                teal: {
                    700: '#006D6E',  // Custom teal color to match the theme
                },
                orange: {
                    600: '#E87C21',  // Custom orange color to match the theme
                },
                gray: {
                    100: '#F5F5F5',  // Light gray background similar to the one used on the site
                }
            },
        },
    },
    plugins: [],
}
