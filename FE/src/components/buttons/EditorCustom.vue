<template >
    <span class="ql-formats">
        <select class="ql-header">
            <option value="1">Heading 1</option>
            <option selected></option>
            <option value="2">Heading 2</option>
            <option value="3">Heading 3</option>
            <option value="4">Heading 4</option>
            <option value="5">Heading 5</option>
            <option value="6">Heading 6</option>
        </select>
    </span>
    <span class="ql-formats">
        <select class="ql-font">
            <option value="sans-serif">Sans Serif</option>
            <option value="serif">Serif</option>
            <option value="arial">Arial</option>
            <option value="monospace">Monospace</option>
        </select>
    </span>
    <span class="ql-formats">
        <button class="ql-list" type="button" value="ordered" v-tooltip.bottom="'List'"></button>
        <button class="ql-list" type="button" value="bullet" v-tooltip.bottom="'List'"></button>
        <button class="ql-indent" value="-1"></button>
        <button class="ql-indent" value="+1"></button>
        <select class="ql-align">
            <option selected></option>
            <option value="right"></option>
            <option value="center"></option>
            <option value="justify"></option>
        </select>
    </span>
    <span class="ql-formats">
        <div class="p-toolbar-group-right">
            <button class="ql-bold" v-tooltip.bottom="'Bold'"></button>
            <button class="ql-italic" v-tooltip.bottom="'Italic'"></button>
            <button class="ql-underline" v-tooltip.bottom="'Underline'"></button>
            <button class="ql-strike"> <i class="pi pi-minus-circle"></i></button>
        </div>
    </span>
    <span class="ql-formats">
        <!-- <button class="ql-table"></button>
        <button class="ql-hr"></button>
        <button class="ql-mathjax"></button> -->
        <button class="ql-script" value="sub"></button>
        <button class="ql-script" value="super"></button>
    </span>
    <span class="ql-formats">
        <select class="ql-color">
            <option value="rgb(0, 0, 0)"></option>
            <option value="rgb(230, 0, 0)"></option>
            <option value="rgb(255, 153, 0)"></option>
            <option value="rgb(255, 255, 0)"></option>
            <option value="rgb(0, 138, 0)"></option>
            <option value="rgb(0, 102, 204)"></option>
            <option value="rgb(153, 51, 255)"></option>
            <option value="rgb(255, 255, 255)"></option>
            <option value="rgb(250, 204, 204)"></option>
            <option value="rgb(255, 235, 204)"></option>
            <option value="rgb(255, 255, 204)"></option>
            <option value="rgb(204, 232, 204)"></option>
            <option value="rgb(204, 224, 245)"></option>
            <option value="rgb(235, 214, 255)"></option>
            <option value="rgb(187, 187, 187)"></option>
            <option value="rgb(102, 185, 102)"></option>
            <option value="rgb(102, 163, 224)"></option>
            <option value="rgb(194, 133, 255)"></option>
            <option value="rgb(136, 136, 136)"></option>
            <option value="rgb(161, 0, 0)"></option>
            <option value="rgb(178, 107, 0)"></option>
            <option value="rgb(178, 178, 0)"></option>
            <option value="rgb(0, 97, 0)"></option>
            <option value="rgb(0, 71, 178)"></option>
            <option value="rgb(107, 36, 178)"></option>
            <option value="rgb(68, 68, 68)"></option>
            <option value="rgb(92, 0, 0)"></option>
            <option value="rgb(102, 61, 0)"></option>
            <option value="rgb(102, 102, 0)"></option>
            <option value="rgb(0, 55, 0)"></option>
            <option value="rgb(0, 41, 102)"></option>
            <option value="rgb(61, 20, 102)"></option>
        </select>
        <select class="ql-background">
            <option value="rgb(0, 0, 0)"></option>
            <option value="rgb(230, 0, 0)"></option>
            <option value="rgb(255, 153, 0)"></option>
            <option value="rgb(255, 255, 0)"></option>
            <option value="rgb(0, 138, 0)"></option>
            <option value="rgb(0, 102, 204)"></option>
            <option value="rgb(153, 51, 255)"></option>
            <option value="rgb(255, 255, 255)"></option>
            <option value="rgb(250, 204, 204)"></option>
            <option value="rgb(255, 235, 204)"></option>
            <option value="rgb(255, 255, 204)"></option>
            <option value="rgb(204, 232, 204)"></option>
            <option value="rgb(204, 224, 245)"></option>
            <option value="rgb(235, 214, 255)"></option>
            <option value="rgb(187, 187, 187)"></option>
            <option value="rgb(102, 185, 102)"></option>
            <option value="rgb(102, 163, 224)"></option>
            <option value="rgb(194, 133, 255)"></option>
            <option value="rgb(136, 136, 136)"></option>
            <option value="rgb(161, 0, 0)"></option>
            <option value="rgb(178, 107, 0)"></option>
            <option value="rgb(178, 178, 0)"></option>
            <option value="rgb(0, 97, 0)"></option>
            <option value="rgb(0, 71, 178)"></option>
            <option value="rgb(107, 36, 178)"></option>
            <option value="rgb(68, 68, 68)"></option>
            <option value="rgb(92, 0, 0)"></option>
            <option value="rgb(102, 61, 0)"></option>
            <option value="rgb(102, 102, 0)"></option>
            <option value="rgb(0, 55, 0)"></option>
            <option value="rgb(0, 41, 102)"></option>
            <option value="rgb(61, 20, 102)"></option>
            </select>
    </span>
    <span class="ql-formats">
        <button class="ql-blockquote"><i class="pi pi-quote-right"></i></button>
        <button class="ql-link"><i class="pi pi-link"></i></button>
        <button class="ql-image"><i class="pi pi-image"></i></button>
        <button class="ql-video"><i class="pi pi-video"></i></button>
        
        <button class="ql-direction" value="rtl"></button>
        <button class="ql-code-block"><i class="pi pi-code"></i></button>
    </span>
    <span class="ql-formats">
        <button class="ql-clean"><i class="pi pi-times"></i></button>
    </span>
</template>