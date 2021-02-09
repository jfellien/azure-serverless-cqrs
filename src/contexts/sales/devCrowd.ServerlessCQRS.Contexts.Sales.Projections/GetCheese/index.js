module.exports = async function (context, req, cheese) {
    
    if(cheese){

        let response = [];

        cheese.forEach(group => {
            group.document.forEach(item =>{
                response.push(item)
            })
        });

        context.res = {
            body: response
        };
    }
}