export const FormatExcel = {
    Header: {
        font: {
            bold: true,
            size: 12,
        },
        fill: {
            type: "pattern",
            patternType: 'solid',
            fgColor: { rgb: 'F7DC6F' },
        },
        border: {
            top: { style: 'thin', color: { auto: 1 } },
            left: { style: 'thin', color: { auto: 1 } },
            bottom: { style: 'thin', color: { auto: 1 } },
            right: { style: 'thin', color: { auto: 1 } },
        },
        alignment: {
            horizontal: 'center',
            vertical: 'center',
            wrapText: true,
        },
    },
    Body: {
        border: {
            top: { style: 'thin', color: { auto: 1 } },
            left: { style: 'thin', color: { auto: 1 } },
            bottom: { style: 'thin', color: { auto: 1 } },
            right: { style: 'thin', color: { auto: 1 } },
        },
        alignment: {
            horizontal: 'center',
            vertical: 'center',
            wrapText: true,
        },
    },
    ElementContent1: {
        border: {
            top: { style: 'thin', color: { auto: 1 } },
            left: { style: 'thin', color: { auto: 1 } },
            bottom: { style: 'thin', color: { auto: 1 } },
            right: { style: 'thin', color: { auto: 1 } },
        },
        alignment: {
            wrapText: true,
        },
    },
    ElementContent2: {

    },
};