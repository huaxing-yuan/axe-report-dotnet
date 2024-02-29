function (node, options, virtualNode) {
    var summary = node.getAttribute("summary");
    var captionElement = node.querySelector("caption");

    var describedby = node.getAttribute("aria-describedby");

    if (summary == null && captionElement == null && describedby) {
        return false;
    } else {
        return true;
    }
}