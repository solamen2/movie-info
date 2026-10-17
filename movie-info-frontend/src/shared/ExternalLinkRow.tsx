import { type ReactNode, useEffect, useRef, useState } from "react";
import "./shared.css";

const COPY_FEEDBACK_MS = 1500;

// Puts the link on the clipboard as a real hyperlink, so that pasting into a
// rich-text editor (Google Docs, Word, email, …) gives a clickable "Link"
// without any editing, while pasting into a plain-text field gives the URL.
// Browsers without the rich clipboard API just get the URL.
async function copyLink(url: string): Promise<void> {
  const anchor = `<a href="${url}">Link</a>`;
  if (
    typeof ClipboardItem === "undefined" ||
    !("write" in navigator.clipboard)
  ) {
    await navigator.clipboard.writeText(url);
    return;
  }
  await navigator.clipboard.write([
    new ClipboardItem({
      "text/html": new Blob([anchor], { type: "text/html" }),
      "text/plain": new Blob([url], { type: "text/plain" }),
    }),
  ]);
}

export function ExternalLinkRowSeparator() {
  return (
    <span
      className="external-link-row-separator"
      role="separator"
      aria-orientation="vertical"
    />
  );
}

interface ExternalLinkRowProps {
  // The external page linked to, or "" when it is unknown: the row then links
  // to searchUrl as "Search Google" instead, or shows a "—" placeholder when
  // there is no searchUrl either, in both cases with no copy button.
  url: string;
  searchUrl?: string;
  // A bold label shown right before the link, for rows that carry no data of
  // their own (e.g. "TV Series IMDB:" or "Wikipedia:").
  label?: string;
  // Whether to offer copying the link as a hyperlink (default true).
  copyButton?: boolean;
  // The copy button's accessible name (default "copy-link"); the rows for the
  // same site share one, e.g. "copy-imdb-link", so tests can tell them apart.
  copyAriaLabel?: string;
  // Data from the external site shown ahead of the link (e.g. the IMDB rating
  // and rank)
  children?: ReactNode;
}

// A detail panel's row for a page on an external site (IMDB, Wikipedia, …):
// an optional label and data, the "Link" itself, and a button to copy it.
function ExternalLinkRow({
  url,
  searchUrl,
  label,
  copyButton = true,
  copyAriaLabel = "copy-link",
  children,
}: ExternalLinkRowProps) {
  const [copyStatus, setCopyStatus] = useState<"idle" | "copied" | "failed">(
    "idle",
  );
  const copyTimeoutRef = useRef<number | undefined>(undefined);

  useEffect(() => {
    return () => {
      window.clearTimeout(copyTimeoutRef.current);
    };
  }, []);

  async function handleCopy() {
    window.clearTimeout(copyTimeoutRef.current);
    try {
      await copyLink(url);
      setCopyStatus("copied");
    } catch {
      setCopyStatus("failed");
    }
    copyTimeoutRef.current = window.setTimeout(() => {
      setCopyStatus("idle");
    }, COPY_FEEDBACK_MS);
  }

  return (
    <p className="external-link-row">
      <span>
        {label && <span className="external-link-row-label">{label}</span>}
        {children}
        {url !== "" ? (
          // The separator and the link stay together on a line (see the CSS)
          <span className="external-link-row-link">
            {children != null && <ExternalLinkRowSeparator />}
            <a href={url} target="_blank" rel="noopener">
              Link
            </a>
          </span>
        ) : searchUrl !== undefined ? (
          <>
            {children != null && <ExternalLinkRowSeparator />}
            <a href={searchUrl} target="_blank" rel="noopener">
              Search Google
            </a>
          </>
        ) : (
          <>{children != null && <ExternalLinkRowSeparator />}—</>
        )}
      </span>
      {copyButton && url !== "" && (
        <button
          type="button"
          aria-label={copyAriaLabel}
          className="copy-button"
          onClick={handleCopy}
        >
          {copyStatus === "copied"
            ? "Copied!"
            : copyStatus === "failed"
              ? "Failed"
              : "Copy"}
        </button>
      )}
    </p>
  );
}

export default ExternalLinkRow;
