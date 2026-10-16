import { expect, vi } from "vitest";

// jsdom has no clipboard: this stands in for navigator.clipboard and
// ClipboardItem, recording what the app writes so tests can read back the
// hyperlink (text/html) and the plain-text fallback (text/plain) flavors.
export function mockClipboard({ reject = false } = {}) {
  const written: Record<string, Blob>[] = [];

  class FakeClipboardItem {
    readonly items: Record<string, Blob>;
    constructor(items: Record<string, Blob>) {
      this.items = items;
    }
  }

  const write = vi.fn((items: FakeClipboardItem[]) => {
    if (reject) return Promise.reject(new Error("denied"));
    written.push(...items.map((item) => item.items));
    return Promise.resolve();
  });

  Object.defineProperty(globalThis, "ClipboardItem", {
    value: FakeClipboardItem,
    configurable: true,
  });
  Object.defineProperty(navigator, "clipboard", {
    value: { write, writeText: vi.fn() },
    configurable: true,
  });

  async function flavor(type: string): Promise<string> {
    expect(written).toHaveLength(1);
    const blob = written[0][type];
    expect(blob).toBeDefined();
    expect(blob.type).toBe(type);
    return blob.text();
  }

  return {
    write,
    copiedHtml: () => flavor("text/html"),
    copiedText: () => flavor("text/plain"),
  };
}
