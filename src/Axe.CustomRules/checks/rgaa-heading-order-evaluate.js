function (node, options, virtualNode) {
    //get all headings from document
    var headings = document.querySelectorAll("h1, h2, h3, h4, h5, h6");

    //get current heading level
    var currentHeadingLevel = parseInt(node.tagName.substring(1));

    //get the previous heading from the array. if node is the first heading, previousHeading will be null
    var previousHeading = headings[headings.indexOf(node) - 1];
    //get the previous heading level
    var previousHeadingLevel = previousHeading ? parseInt(previousHeading.tagName.substring(1)) : null;

    if (currentHeadingLevel > previousHeadingLevel) {
        if (currentHeadingLevel - previousHeadingLevel > 1) {
            //there is a gap between the current heading and the previous heading
            //for example, the current heading is h3 and the previous heading is h1
            this.data({ node });
            return false;
        }
    }

}