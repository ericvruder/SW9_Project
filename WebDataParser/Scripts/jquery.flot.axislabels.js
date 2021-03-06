(function($){var options={};function init(plot){var secondPass=false;var defaultPadding=2;plot.hooks.draw.push(function(plot,ctx){if(!secondPass){$.each(plot.getAxes(),function(axisName,axis){var opts=axis.options||plot.getOptions()[axisName];if(!opts||!opts.axisLabel)
return;var w,h;var padding=opts.axisLabelPadding===undefined?defaultPadding:opts.axisLabelPadding;if(opts.axisLabelUseCanvas!=false)
opts.axisLabelUseCanvas=true;if(opts.axisLabelUseCanvas){if(!opts.axisLabelFontSizePixels)
opts.axisLabelFontSizePixels=14;if(!opts.axisLabelFontFamily)
opts.axisLabelFontFamily='sans-serif';w=opts.axisLabelFontSizePixels+ padding;h=opts.axisLabelFontSizePixels+ padding;}else{var elem=$('<div class="axisLabels" style="position:absolute;">'+ opts.axisLabel+'</div>');plot.getPlaceholder().append(elem);w=elem.outerWidth(true);h=elem.outerHeight(true);elem.remove();}
if(axisName.charAt(0)=='x')
axis.labelHeight+=h;else
axis.labelWidth+=w;opts.labelHeight=axis.labelHeight;opts.labelWidth=axis.labelWidth;});secondPass=true;plot.setupGrid();plot.draw();}else{$.each(plot.getAxes(),function(axisName,axis){var opts=axis.options||plot.getOptions()[axisName];if(!opts||!opts.axisLabel)
return;if(opts.axisLabelUseCanvas){var ctx=plot.getCanvas().getContext('2d');ctx.save();ctx.font=opts.axisLabelFontSizePixels+'px '+
opts.axisLabelFontFamily;var width=ctx.measureText(opts.axisLabel).width;var height=opts.axisLabelFontSizePixels;var x,y,angle=0;if(axisName=='xaxis'){x=plot.getPlotOffset().left+ plot.width()/2 - width/2;
y=plot.getCanvas().height- height*0.28;}else if(axisName=='x2axis'){x=plot.getPlotOffset().left+ plot.width()/2 - width/2;
y=height;}else if(axisName=='yaxis'){x=height*0.72;y=plot.getPlotOffset().top+ plot.height()/2 + width/2;
angle=-Math.PI/2;}else if(axisName=='y2axis'){x=plot.getPlotOffset().left+ plot.width()+ plot.getPlotOffset().right- height*0.72;y=plot.getPlotOffset().top+ plot.height()/2 - width/2;
angle=Math.PI/2;}
ctx.translate(x,y);ctx.rotate(angle);ctx.fillText(opts.axisLabel,0,0);ctx.restore();}else{plot.getPlaceholder().find('#'+ axisName+'Label').remove();var elem=$('<div id="'+ axisName+'Label" " class="axisLabels" style="position:absolute;">'+ opts.axisLabel+'</div>');plot.getPlaceholder().append(elem);if(axisName=='xaxis'){elem.css('left',plot.getPlotOffset().left+ plot.width()/2 - elem.outerWidth()/2 + 'px');
elem.css('bottom','0px');}else if(axisName=='x2axis'){elem.css('left',plot.getPlotOffset().left+ plot.width()/2 - elem.outerWidth()/2 + 'px');
elem.css('top','0px');}else if(axisName=='yaxis'){elem.css('top',plot.getPlotOffset().top+ plot.height()/2 - elem.outerHeight()/2 + 'px');
elem.css('left','0px');}else if(axisName=='y2axis'){elem.css('top',plot.getPlotOffset().top+ plot.height()/2 - elem.outerHeight()/2 + 'px');
elem.css('right','0px');}}});}});}
$.plot.plugins.push({init:init,options:options,name:'axisLabels',version:'1.0'});})(jQuery);