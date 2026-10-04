import { type ReactNode, useEffect, useRef, useState } from "react";
import "./shared.css";

const COPY_FEEDBACK_MS = 1500;

export function ImdbRowSeparator() {
  return (
    <span
      className="imdb-row-separator"
      role="separator"
      aria-orientation="vertical"
    />
  );
}

interface ImdbRowProps {
  // The IMDB page linked to, or "" when the IMDB ID is unknown: the row then
  // links to searchUrl as "Search Google" instead, with no copy button.
  imdbUrl: string;
  searchUrl?: string;
  // A bold label shown right before the link, for rows that carry no data of
  // their own (e.g. "TV Series IMDB:").
  label?: string;
  // Whether to offer copying the link as an HTML anchor (default true).
  copyButton?: boolean;
  // The IMDB data shown ahead of the link (rating, rank, etc.)
  children?: ReactNode;
}

function ImdbRow({
  imdbUrl,
  searchUrl,
  label,
  copyButton = true,
  children,
}: ImdbRowProps) {
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
      await navigator.clipboard.writeText(`<a href="${imdbUrl}">Link</a>`);
      setCopyStatus("copied");
    } catch {
      setCopyStatus("failed");
    }
    copyTimeoutRef.current = window.setTimeout(() => {
      setCopyStatus("idle");
    }, COPY_FEEDBACK_MS);
  }

  return (
    <p className="imdb-row">
      <span>
        {label && <span className="imdb-row-label">{label}</span>}
        {children != null && (
          <>
            {children}
            <ImdbRowSeparator />
          </>
        )}
        {imdbUrl !== "" ? (
          <a href={imdbUrl} target="_blank" rel="noopener">
            Link
          </a>
        ) : (
          <a href={searchUrl} target="_blank" rel="noopener">
            Search Google
          </a>
        )}
      </span>
      {copyButton && imdbUrl !== "" && (
        <button
          type="button"
          aria-label="copy-imdb-link"
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

export default ImdbRow;
