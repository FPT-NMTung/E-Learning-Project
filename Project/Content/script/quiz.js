const activeAns = (name, el) => {
    console.log(`name: ${name} | id: ${el}`)
    
    $(`.${name}`).each(function(i, obj) {
        if ($(obj).hasClass('active-ans')) {
            $(obj).removeClass('active-ans')
        }
    });

    $(el).addClass('active-ans')
}