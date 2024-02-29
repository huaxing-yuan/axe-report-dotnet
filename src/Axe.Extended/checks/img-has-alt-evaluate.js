function (node, options, virtualNode) {
    var alt = node.getAttribute("alt");
    var title = node.getAttribute("title");
    var ariaLabel = node.getAttribute("aria-label");
    var ariaLabelledBy = node.getAttribute("aria-labelledby");

    if (alt == null && title == null && ariaLabel == null && ariaLabelledBy == null) {
        return false;
    } else {
        return true;
    }
}