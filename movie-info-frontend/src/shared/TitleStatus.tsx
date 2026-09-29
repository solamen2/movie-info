interface TitleStatusProps {
  status: string | null;
  // The status that goes without saying (e.g. "Released" for a movie, and "Ended" for a TV series), and is
  // therefore not shown.
  usualStatus: string;
}

// A movie's or TV series' status, shown next to its title when it is anything
// out of the ordinary, e.g. "(in production)". It goes beside the title's
// heading in a `.detail-title-row` rather than inside it, so the heading is
// the title alone (for anything that looks the heading up by name).
function TitleStatus({ status, usualStatus }: TitleStatusProps) {
  const shownStatus = status?.trim().toLowerCase() ?? "";
  if (shownStatus === "" || shownStatus === usualStatus.toLowerCase()) {
    return null;
  }
  return (
    <i className="detail-title-status" data-testid="title-status">
      ({shownStatus})
    </i>
  );
}

export default TitleStatus;
