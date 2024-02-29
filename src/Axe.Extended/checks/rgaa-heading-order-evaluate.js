function (node, options, virtualNode) {

    //get all headings from document
    var nodeList = document.querySelectorAll("h1, h2, h3, h4, h5, h6");
    var headings = Array.from(nodeList);



    //get the previous heading from the array. if node is the first heading, previousHeading will be null
    var index = headings.indexOf(node);
    if (index <= 0) return true;
    if (index > headings.length - 1) return true;

    //get current heading level
    var currentHeadingLevel = parseInt(node.tagName.substring(1));
    var previousHeading = headings[index - 1];
    //get the previous heading level
    var previousHeadingLevel = previousHeading ? parseInt(previousHeading.tagName.substring(1)) : null;

    if (currentHeadingLevel > previousHeadingLevel && currentHeadingLevel - previousHeadingLevel > 1) {
        //there is a gap between the current heading and the previous heading
        //for example, the current heading is h3 and the previous heading is h1
        //created an array containing previeus heading
        var array = [previousHeading];
        this.relatedNodes(array);
        return false;
    }
    return true;
}