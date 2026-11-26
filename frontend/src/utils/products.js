export function productImage(product) {
  const raw = product?.image || product?.images || "";
  if (Array.isArray(raw)) return raw[0] || "";
  if (typeof raw !== "string") return "";
  if (raw.startsWith("[")) {
    try {
      return JSON.parse(raw)[0] || "";
    } catch {
      return "";
    }
  }
  return raw.split("|")[0].trim();
}

export function priceLabel(product) {
  const amount = Number(product?.price);
  if (!Number.isFinite(amount)) return "Цена не указана";
  const currency = product.currency || "RUB";
  try {
    return new Intl.NumberFormat("ru-RU", {
      style: "currency",
      currency,
      minimumFractionDigits: 0,
      maximumFractionDigits: 2,
    }).format(amount);
  } catch {
    return `${amount.toLocaleString("ru-RU")} ${currency}`;
  }
}
