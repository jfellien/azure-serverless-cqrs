module.exports = async function (context, req, tomatoes) {
    
    if(tomatoes){

        let response = [];

        tomatoes.forEach(group => {
            group.document.forEach(item =>{
                response.push(item)
            })
        });

        context.res = {
            body: response
        };
    }
}