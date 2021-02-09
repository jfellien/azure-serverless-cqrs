module.exports = async function (context, req, pastes) {
    
    if(pastes){

        let response = [];

        pastes.forEach(group => {
            group.document.forEach(item =>{
                response.push(item)
            })
        });

        context.res = {
            body: response
        };
    }
}