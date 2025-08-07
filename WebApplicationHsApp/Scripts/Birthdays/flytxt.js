var count=0;
var length;
var mar=250;
var top1=100;
var myloop;
var message="Wish You.Many.More.Happy Returns.of the day";
var message_array=message.split('.');
var slashn=0
// if ((!$.browser.webkit) && (!$.browser.msie)) {
//         //
//var MyAudio=document.getElementById("rhySound");
//  MyAudio.preload="auto";         

//}
 
//$(document).ready(function(){
//         var sds = document.getElementById("dum");
//    if(sds == null){
//        alert("You are using a free package.\n You are not allowed to remove the tag.\n");
//    }
//    var sdss = document.getElementById("dumdiv");
//    if(sdss == null){
//        alert("You are using a free package.\n You are not allowed to remove the tag.\n");
//    }
//    if(sds!= null){
//	 if (navigator.appName == 'Microsoft Internet Explorer')
//  {
//      slashn=80;
//}
//else
// {
	
//	 slashn=80;
// }
	
//	//alert("hai");
//	length=message_array.length;
//	for(var i=0;i<length;i++)
//	{
//		$('div.container').append("<span style='opacity:0;font-size:2em;position:absolute;width:400px;' id='"+i+"'></span><br /><br />");
		
		
//	}
//	start();
//	$("#play").hide();
//    }
//});

function play()
{
	$("#pause").show();
	$("#play").hide();
	count=0;
	mar=250;
	top1=100;
	cc=0;
	for(var j=0;j<length;j++)
	{
		$(".container #"+j+"").html(message_array[j]).animate({opacity:0},1000);
		$(".container #"+j+"").html(message_array[j]).animate({marginLeft:0},100);
		$(".container #"+j+"").html(message_array[j]).animate({ top:0},1000);
	}
	start();
	
}

function pause()
{
	clearInterval(myloop);
         if ((!$.browser.webkit) && (!$.browser.msie)) {
         MyAudio.pause();
         }
	$("#pause").hide();
	$("#play").show();
}



function start()
{
	myloop=setInterval(function(){
		change(count)
	},3500);
}	
var cc=0;
var mar1=0;
function change(myunit)
{
 if ((!$.browser.webkit) && (!$.browser.msie)) {
	 MyAudio.play();
 }
	$(".container #"+count+"").html(message_array[myunit]).animate({opacity:1, top:top1},1000);
	$(".container #"+count+"").html(message_array[myunit]).animate({opacity:1, marginLeft:mar},1000);
	var mov=$("#"+count+"").width();
	if(count==3)
	  mar=mar1;
	if(count==2)
	{
	  mar1=mar+50-cc;	
	  mar=mar-cc;
	  
	}
	else
	  mar=mar+20-cc;
	
	top1 =top1+slashn;
	//alert(mar);
	count++;
	cc=cc+15;
	if(count==length)
	{
		
		clearInterval(myloop);
		$("#pause").hide();
		$("#play").show();
		
	}
}

    