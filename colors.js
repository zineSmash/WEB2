var Links = {
  setColor:function(color){
  var alist = document.querySelectorAll('a');
    var i=0;
    while(i<alist.length){
      alist[i].style.color = color;
      i++;
    }
  }
}

var body = {
  setColor:function(target, color){
  target.style.color = color;
  },
  setBackgroundColor:function(target, color){
  target.style.backgroundColor = color;
  }
}

function nightdayHandler(self){
  var target = document.querySelector('body');
  if(self.value === 'day'){  
    body.setBackgroundColor(target, 'white');
    body.setColor(target,'black');
    self.value='night';

    Links.setColor('black');
    document.querySelector("h3").style.bordercolor ='black';
  }
  else{
    body.setBackgroundColor(target, 'black');;
    body.setColor(target,'white');
    self.value = 'day';

    Links.setColor('powderblue');
    document.querySelector("h3").style.bordercolor ='white';
  }
}