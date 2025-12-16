let selectedMembers = [];

function renderMembers() {
    let html = "";
    selectedMembers.forEach((m, i) => {
        html += `
            <span class="member-tag">
                ${m.name}
                <span onclick="removeMember(${i})">✖</span>
            </span>
        `;
    });

    $("#selectedMembers").html(html);
    $("#MemberIds").val(selectedMembers.map(x => x.id).join(","));
}

function removeMember(index) {
    selectedMembers.splice(index, 1);
    renderMembers();
}

// ================= DATE =================
function calculateDays() {
    const startVal = $("#StartDate").val();
    const endVal = $("#EndDate").val();

    if (!startVal || !endVal) return;

    const start = new Date(startVal);
    const end = new Date(endVal);

    if (end < start) {
        $("#dateError").show();
        $("#TotalDays").val(0);
        return;
    }

    $("#dateError").hide();
    const days = Math.floor((end - start) / (1000 * 60 * 60 * 24)) + 1;
    $("#TotalDays").val(days);
}

// ================= ITEM TOTAL =================
function recalcAmount() {
    let total = 0;

    $("#itemsTable tbody tr").each(function () {
        const qty = parseFloat($(this).find(".quantity").val()) || 0;
        const price = parseFloat($(this).find(".price").val()) || 0;
        const rowTotal = qty * price;

        $(this).find(".item-total").text(rowTotal.toFixed(2));
        total += rowTotal;
    });

    $("#BazarAmount").val(total.toFixed(2));
}

// ================= INIT =================
$(document).ready(function () {

    // ADD MEMBER
    $("#memberSelect").on("change", function () {
        const id = $(this).val();
        const name = $("#memberSelect option:selected").text();

        if (!id || selectedMembers.some(x => x.id === id)) {
            $(this).val("");
            return;
        }

        selectedMembers.push({ id, name });
        renderMembers();
        $(this).val("");
    });

    // CREATE MODE
    if (window.pageData.isNew) {
        const today = new Date().toISOString().split("T")[0];
        $("#BazarDate").val(today);
        $("#StartDate").val(today);
    }

    // DATE EVENTS
    $("#BazarDate").on("change", function () {
        $("#StartDate").val(this.value);
        calculateDays();
    });

    $("#EndDate").on("change", calculateDays);

    // ITEMS
    $("#addItem").click(function () {
        const index = $("#itemsTable tbody tr").length;
        $("#itemsTable tbody").append(`
            <tr>
                <td><input name="Items[${index}].ProductName" class="form-control" /></td>
                <td><input name="Items[${index}].Quantity" class="form-control quantity" /></td>
                <td><input name="Items[${index}].Price" class="form-control price" /></td>
                <td class="item-total">0.00</td>
                <td><button type="button" class="btn-remove remove-item">Remove</button></td>
            </tr>
        `);
    });

    $(document).on("input", ".quantity, .price", recalcAmount);
    $(document).on("click", ".remove-item", function () {
        $(this).closest("tr").remove();
        recalcAmount();
    });

    // EDIT MODE MEMBERS LOAD
    if (window.pageData.members?.length) {
        selectedMembers = window.pageData.members;
        renderMembers();
    }

    recalcAmount();
    calculateDays();
});
